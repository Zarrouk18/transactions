using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Transactions.Application.Auth;
using Transactions.Application.DTOs;
using Transactions.Application.Email;
using Transactions.Application.Settings;
using Transactions.Domain;
using Transactions.Infrastructure.Email;





namespace Transactions.Infrastructure
{
    public class AuthService : IAuthService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        

        public AuthService(
            JwtSettings jwtSettings,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
             IEmailService emailService)
        {
            _jwtSettings = jwtSettings;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
        }


        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
                throw new UnauthorizedAccessException("Utilisateur introuvable");

            // Vérifie si l'utilisateur est bloqué
            if (await _userManager.IsLockedOutAsync(user))
            {
                var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                var remaining = lockoutEnd?.UtcDateTime.Subtract(DateTime.UtcNow);

                throw new UnauthorizedAccessException($"Compte bloqué. Réessayez dans {remaining?.Minutes} minute(s).");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!isPasswordValid)
            {
                //  Incrémente le compteur d'échecs
                await _userManager.AccessFailedAsync(user);

                throw new UnauthorizedAccessException("Mot de passe incorrect.");
            }

            //  Si mot de passe correct, reset du compteur
            await _userManager.ResetAccessFailedCountAsync(user);

            var roles = await _userManager.GetRolesAsync(user);

            var accessToken = GenerateAccessToken(user.UserName, roles.FirstOrDefault());
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);

            await _userManager.UpdateAsync(user);

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }


        public async Task<AuthResponse> RefreshAsync(RefreshRequest request)
        {
            //  Look for the user whose refresh token matches
            var user = _userManager.Users.FirstOrDefault(u =>
                u.RefreshToken == request.RefreshToken &&
                u.RefreshTokenExpiryTime > DateTime.UtcNow);

            if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow || user.RefreshTokenExpiryTime == DateTime.MinValue)
                throw new SecurityTokenException("Refresh token invalide ou expiré.");

            var roles = await _userManager.GetRolesAsync(user);

            var newAccessToken = GenerateAccessToken(user.UserName, roles.FirstOrDefault());
            var newRefreshToken = GenerateRefreshToken();

            
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);

            await _userManager.UpdateAsync(user);

            return new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }


        private string GenerateAccessToken(string username, string? role)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, username)
    };

            if (!string.IsNullOrEmpty(role))
                claims.Add(new Claim(ClaimTypes.Role, role.ToUpper()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
        private string GenerateRandomPassword()
        {
            return $"P@ss{Guid.NewGuid().ToString("N").Substring(0, 8)}!";
        }
        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _userManager.FindByNameAsync(request.Username);
            if (existingUser != null)
                throw new InvalidOperationException("Ce nom d'utilisateur est déjà utilisé.");

            //  Générer un mot de passe aléatoire si non fourni
            var generatedPassword = string.IsNullOrWhiteSpace(request.Password)
                ? GenerateRandomPassword()
                : request.Password;

            var user = new ApplicationUser
            {
                UserName = request.Username,
                LockoutEnabled = true,
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(user, generatedPassword);
            if (!result.Succeeded)
                throw new Exception("Échec de la création de l'utilisateur : " +
                    string.Join("; ", result.Errors.Select(e => e.Description)));

            // Vérifie que le rôle existe
            if (!await _roleManager.RoleExistsAsync(request.Role))
                throw new InvalidOperationException($"Le rôle {request.Role} n'existe pas.");

            await _userManager.AddToRoleAsync(user, request.Role);

            var accessToken = GenerateAccessToken(user.UserName, request.Role);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
            await _userManager.UpdateAsync(user);

            // Envoi de l'email de bienvenue
            await _emailService.SendWelcomeEmailAsync(user.UserName, user.Email, generatedPassword);


            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }


        public async Task LogoutAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                throw new SecurityTokenException("Utilisateur introuvable");

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            await _userManager.UpdateAsync(user);
        }
       


    }
}

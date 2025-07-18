using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Transactions.Application.Auth;
using Transactions.Application.DTOs;
using Transactions.Application.Settings;
using Transactions.Domain;

namespace Transactions.Infrastructure
{
    public class AuthService : IAuthService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        private static readonly Dictionary<string, string> RefreshTokens = new();

        public AuthService(
            JwtSettings jwtSettings,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _jwtSettings = jwtSettings;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
                throw new UnauthorizedAccessException("Utilisateur introuvable");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
                throw new UnauthorizedAccessException("Mot de passe incorrect");

            var roles = await _userManager.GetRolesAsync(user);

            var accessToken = GenerateAccessToken(user.UserName, roles.FirstOrDefault());
            var refreshToken = GenerateRefreshToken();

            RefreshTokens[user.UserName] = refreshToken;

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<AuthResponse> RefreshAsync(RefreshRequest request)
        {
            var username = RefreshTokens.FirstOrDefault(x => x.Value == request.RefreshToken).Key;

            if (string.IsNullOrEmpty(username))
                throw new SecurityTokenException("Refresh token invalide");

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                throw new SecurityTokenException("Utilisateur introuvable");

            var roles = await _userManager.GetRolesAsync(user);

            var newAccessToken = GenerateAccessToken(user.UserName, roles.FirstOrDefault());
            var newRefreshToken = GenerateRefreshToken();

            RefreshTokens[username] = newRefreshToken;

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
        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _userManager.FindByNameAsync(request.Username);
            if (existingUser != null)
                throw new InvalidOperationException("Ce nom d'utilisateur est déjà utilisé.");

            var user = new ApplicationUser
            {
                UserName = request.Username,
                //Email = request.Email // si tu veux stocker l'email aussi
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                throw new Exception("Échec de la création de l'utilisateur : " +
                    string.Join("; ", result.Errors.Select(e => e.Description)));

            // Vérifie que le rôle existe
            if (!await _roleManager.RoleExistsAsync(request.Role))
                throw new InvalidOperationException($"Le rôle {request.Role} n'existe pas.");

            await _userManager.AddToRoleAsync(user, request.Role);

            var accessToken = GenerateAccessToken(user.UserName, request.Role);
            var refreshToken = GenerateRefreshToken();

            RefreshTokens[user.UserName] = refreshToken;

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }


    }
}

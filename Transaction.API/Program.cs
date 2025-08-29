using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Net.Mail;
using System.Text;
using Transactions.Application;
using Transactions.Application.Auth;
using Transactions.Application.Repositories;
using Transactions.Application.Services.TransactionsServices;
using Transactions.Application.Settings;
using Transactions.Domain;
using Transactions.Infrastructure;
using Transactions.Infrastructure.Email;
using Transactions.Infrastructure.Models;
using Transactions.Infrastructure.Repositories;
using Swashbuckle.AspNetCore.Swagger;
using Transactions.Application.Email;
using Transactions.Application.Services.Users;

//using MySmtpSettings = Transactions.Application.Settings.SmtpSettings;




var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Main API",
        Version = "v1",
        Description = "API for transaction and user management"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT using Bearer. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
   // c.SerializeAsV2 = false;
});


//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v2.0", new OpenApiInfo { Title = "Main API v2.0", Version = "v2.0" });

//    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
//        Name = "Authorization",
//        In = ParameterLocation.Header,
//        Type = SecuritySchemeType.ApiKey,
//        Scheme = "Bearer"
//    });

//    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                },
//                Scheme = "oauth2",
//                Name = "Bearer",
//                In = ParameterLocation.Header,
//            },
//            new List<string>()
//        }
//    });
//});






// Register DbContext with connection string
builder.Services.AddDbContext<TransactionDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"))
        .EnableSensitiveDataLogging() 
        .LogTo(Console.WriteLine, LogLevel.Information));

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.AddSingleton(jwtSettings);

var lockoutSettings = builder.Configuration
    .GetSection("IdentityOptions:Lockout")
    .Get<LockoutSettings>();
builder.Services.Configure<LockoutSettings>(
    builder.Configuration.GetSection("IdentityOptions:Lockout"));


builder.Services.Configure<CompanySettings>(
    builder.Configuration.GetSection("Company"));

builder.Services.Configure<SmtpSettings>(
    builder.Configuration.GetSection("SmtpSettings"));




builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(lockoutSettings.DefaultLockoutTimeSpanInMinutes);
    options.Lockout.MaxFailedAccessAttempts = lockoutSettings.MaxFailedAccessAttempts;
    options.Lockout.AllowedForNewUsers = lockoutSettings.AllowedForNewUsers;
})
.AddEntityFrameworkStores<TransactionDbContext>()
.AddDefaultTokenProviders();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    };
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy => policy.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});




// Register services and repositories
builder.Services.AddScoped<ITransactionsService, TransactionsService>();
builder.Services.AddScoped<ITransactionsRepo, TransactionsRepo>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<EmailTemplateService>();
builder.Services.AddScoped<IUsersService, UsersService>();








var app = builder.Build();

// Swagger only in development
// Then your UI config:
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Main API V1 (Swagger 2.0)");
    c.RoutePrefix = string.Empty;
});
// Uncomment the following lines to enable Swagger v2.0
//app.UseSwagger();
//app.UseSwaggerUI(c =>
//{
//    c.SwaggerEndpoint("/swagger/v2.0/swagger.json", "Main API v2.0");
//    c.DocumentTitle = "Gateway Solutions";
//    c.DocExpansion(DocExpansion.None);
//    c.RoutePrefix = string.Empty;
//});



app.UseRouting();
app.UseCors("AllowAngularApp");
// Disable HTTPS redirection temporarily for testing
// Comment or remove this line to avoid: "Failed to determine the https port for redirect"
// app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

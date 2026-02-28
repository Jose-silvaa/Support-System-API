using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Support_System_API.Data;
using Support_System_API.Domain;
using Support_System_API.Domain.Enums;
using Support_System_API.Services.Auth;
using Support_System_API.Services.Interfaces;
using Support_System_API.Services.Interfaces.Ticket;
using Support_System_API.Services.Ticket;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrEmpty(jwtKey))
    throw new Exception("JWT Key not configured!");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey!))
        };
    });


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!context.Users.Any(u => u.Role.ToString() == "Admin"))
    {
        var passwordHasher = new PasswordHasher<User>();
        var adminPassword = builder.Configuration["Admin:Password"];
        var adminEmail = builder.Configuration["Admin:Email"];
        
        if (string.IsNullOrWhiteSpace(adminEmail) ||
            string.IsNullOrWhiteSpace(adminPassword))
        {
            throw new Exception("Admin credentials not configured.");
        }
        
        var admin = new User
        {
            Email = adminEmail,
            Role = UserRole.Admin, 
        };
        
        admin.PasswordHash = passwordHasher.HashPassword(admin, adminPassword);
        
        context.Users.Add(admin);
        context.SaveChanges();
    }
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

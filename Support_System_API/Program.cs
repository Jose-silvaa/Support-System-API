using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;
using Support_System_API.Data;
using Support_System_API.Domain.Entities;
using Support_System_API.Domain.Enums;
using Support_System_API.Infrastructure.Logging;
using Support_System_API.Services.Auth;
using Support_System_API.Services.Interfaces;
using Support_System_API.Services.Interfaces.Ticket;
using Support_System_API.Services.Interfaces.TicketHistory;
using Support_System_API.Services.Interfaces.User;
using Support_System_API.Services.Ticket;
using Support_System_API.Services.TicketHistory;
using Support_System_API.Services.User;

LoggingConfiguration.Configure();

Log.Information("Starting application");

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "System Support Ticket", 
        Version = "v1",
        Description = "An ASP.NET Core Web API for managing support tickets"
    });
    
    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Digite: Bearer {seu token}"
    });
    
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("bearer", document)] = []
    });
    
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddScoped<ITicketHistoryService, TicketHistoryService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddMemoryCache();
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "https://support-system-frontend-delta.vercel.app")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "System Support Ticket");
    options.RoutePrefix = string.Empty;
});


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();

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
app.UseCors("AllowFrontend");

app.MapControllers();

app.Run();

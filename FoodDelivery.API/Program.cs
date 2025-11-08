using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using FoodDelivery.API.Data;
using FoodDelivery.API.Models;
using FoodDelivery.API.Services;
using System;

var builder = WebApplication.CreateBuilder(args);

// EF Core - Validate connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// For Development, allow LocalDB if connection string not configured
if (builder.Environment.IsDevelopment() && 
    (string.IsNullOrEmpty(connectionString) || 
     connectionString.Contains("YOUR_") || 
     connectionString.Contains("Password=YOUR_PASSWORD")))
{
    // Use LocalDB for development if not configured
    connectionString = "Server=(localdb)\\mssqllocaldb;Database=FoodDeliveryDB;Trusted_Connection=True;MultipleActiveResultSets=true";
    Console.WriteLine("⚠️  Using default LocalDB connection for development. Configure your connection string in User Secrets or appsettings.Development.json");
}
else if (string.IsNullOrEmpty(connectionString) || 
         connectionString.Contains("YOUR_") || 
         connectionString.Contains("Password=YOUR_PASSWORD"))
{
    throw new InvalidOperationException(
        "Database connection string is not configured. Please set ConnectionStrings:DefaultConnection " +
        "in appsettings.json, appsettings.Development.json, User Secrets, or environment variables.");
}

builder.Services.AddDbContext<FoodDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<FoodDbContext>()
    .AddDefaultTokenProviders();

// JWT - Validate configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var jwtSecret = jwtSettings["Secret"];

// For Development, generate a temporary secret if not configured
if (builder.Environment.IsDevelopment() && 
    (string.IsNullOrEmpty(jwtSecret) || jwtSecret.Contains("YOUR_") || jwtSecret.Length < 32))
{
    // Generate a temporary secret for development
    jwtSecret = "Development-Temporary-JWT-Secret-Key-Minimum-32-Characters-Required-For-Security-12345";
    Console.WriteLine("⚠️  Using temporary JWT secret for development. Configure JwtSettings:Secret in User Secrets or appsettings.Development.json for production.");
}
else if (string.IsNullOrEmpty(jwtSecret) || jwtSecret.Contains("YOUR_") || jwtSecret.Length < 32)
{
    throw new InvalidOperationException(
        "JWT Secret is not configured. Please set JwtSettings:Secret in appsettings.json or environment variables. " +
        "Minimum 32 characters required. For development, use User Secrets or update appsettings.Development.json");
}

var key = Encoding.UTF8.GetBytes(jwtSecret);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddScoped<JwtTokenService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FoodDelivery API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }});
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Seed roles & admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.InitializeAsync(services);
}

app.Run();

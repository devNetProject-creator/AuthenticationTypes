//using FoodDelivery.API.Data;
using FoodDelivery.MVC.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//// EF Core
//builder.Services.AddDbContext<FoodDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddHttpContextAccessor();

// Add authentication
builder.Services.AddAuthentication(options =>
{
    // Default scheme for authN (e.g., MVC pages) is cookie
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

    // These can be overridden per controller/endpoint
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie() // for Google & UI-based login
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    var clientId = builder.Configuration["Authentication:Google:ClientId"];
    var clientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    
    // Google OAuth is optional - only configure if values are provided
    if (!string.IsNullOrEmpty(clientId) && !clientId.Contains("YOUR_") &&
        !string.IsNullOrEmpty(clientSecret) && !clientSecret.Contains("YOUR_"))
    {
        options.ClientId = clientId;
        options.ClientSecret = clientSecret;
    }
    
    options.ClaimActions.MapJsonKey("picture", "picture", "url");
})
.AddJwtBearer("JwtBearer", options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var jwtSecret = jwtSettings["Secret"];
    
    // JWT is optional for MVC (used when calling API)
    if (!string.IsNullOrEmpty(jwtSecret) && !jwtSecret.Contains("YOUR_") && jwtSecret.Length >= 32)
    {
        var key = Encoding.UTF8.GetBytes(jwtSecret);
        
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
    }
});

// Register HttpClient for API communication
builder.Services.AddHttpClient<ApiService>();
builder.Services.AddHttpClient<IUserService, UserService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();

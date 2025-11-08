using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using FoodDelivery.API.Models;

namespace FoodDelivery.API.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = ["Admin", "Customer", "Delivery", "Manager"];
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Get configuration for seed data passwords
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            
            // Create default admin
            var adminEmail = "admin@food.com";
            var adminPassword = configuration["SeedData:AdminPassword"] ?? "Admin@123"; // Default for development
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                var user = new ApplicationUser { UserName = adminEmail, Email = adminEmail, FullName = "Super Admin" };
                var result = await userManager.CreateAsync(user, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }

            // Create default customer
            var customerEmail = "customer@food.com";
            var customerPassword = configuration["SeedData:CustomerPassword"] ?? "Customer@123"; // Default for development
            var customer = await userManager.FindByEmailAsync(customerEmail);
            if (customer == null)
            {
                var user = new ApplicationUser { UserName = customerEmail, Email = customerEmail, FullName = "Customer" };
                var result = await userManager.CreateAsync(user, customerPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Customer");
                }
            }
        }
    }
}

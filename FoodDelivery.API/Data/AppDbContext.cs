
using Microsoft.EntityFrameworkCore;
using FoodDelivery.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace FoodDelivery.API.Data
{
    public class FoodDbContext : IdentityDbContext<ApplicationUser>
    {
        public FoodDbContext(DbContextOptions<FoodDbContext> options) : base(options) { }

        // Add your custom entities here if needed
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

    }
}

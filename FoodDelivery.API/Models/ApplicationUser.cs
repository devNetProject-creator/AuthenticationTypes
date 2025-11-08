using Microsoft.AspNetCore.Identity;

namespace FoodDelivery.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }

        // Add this property to track if it's a Google-authenticated user
        public bool IsGoogleUser { get; set; }
    }
}

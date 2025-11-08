namespace FoodDelivery.API.Models.DTOs

{
    using System.ComponentModel.DataAnnotations;

    public class CreateUserDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }
        public bool IsGoogleUser { get; set; }
    }
}

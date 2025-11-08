namespace FoodDelivery.MVC.Dto
{
    using System.ComponentModel.DataAnnotations;

    public class CreateUserDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
    }

}

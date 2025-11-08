namespace FoodDelivery.MVC.Dto
{
    using System.ComponentModel.DataAnnotations;

    public class UpdateUserRoleDto
    {
        [Required]
        public string Role { get; set; }
    }

}

using FoodDelivery.MVC.Models;

namespace FoodDelivery.MVC.Services
{
    public interface IUserService
    {
        Task<List<UserViewModel>> GetAllUsersAsync();
        Task<UserViewModel> GetUserByIdAsync(string id);

        Task<bool> CreateUserAsync(UserViewModel model);
        Task<bool> UpdateUserAsync(UserViewModel model);
        Task<bool> DeleteUserAsync(string id);
        Task<List<string>> GetAllRolesAsync();
    }
}

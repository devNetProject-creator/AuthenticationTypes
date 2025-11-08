using FoodDelivery.MVC.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace FoodDelivery.MVC.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(HttpClient client, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _client = client;
            _config = config;
            _client.BaseAddress = new Uri(_config["ApiSettings:BaseUrl"]);
            _httpContextAccessor = httpContextAccessor;
        }

        private void AddJwtToken()
        {
            var token = _httpContextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(c => c.Type == "AccessToken")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<UserViewModel>> GetAllUsersAsync()
        {
            AddJwtToken();
            var res = await _client.GetAsync("api/users");
            res.EnsureSuccessStatusCode();
            var json = await res.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<UserViewModel>>(json); //Get all users
        }

        public async Task<UserViewModel> GetUserByIdAsync(string id)
        {
            AddJwtToken();
            var res = await _client.GetAsync($"api/users/{id}");
            res.EnsureSuccessStatusCode();
            var json = await res.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserViewModel>(json);
        }

        public async Task<bool> CreateUserAsync(UserViewModel model)
        {
            AddJwtToken();
            var res = await _client.PostAsJsonAsync("api/users", model);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateUserAsync(UserViewModel model)
        {
            AddJwtToken();
            var res = await _client.PutAsJsonAsync($"api/users/{model.Id}", model);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            AddJwtToken();
            var res = await _client.DeleteAsync($"api/users/{id}");
            return res.IsSuccessStatusCode;
        }

        public async Task<List<string>> GetAllRolesAsync()
        {
            AddJwtToken();
            var res = await _client.GetAsync("api/users/roles");
            res.EnsureSuccessStatusCode();
            var json = await res.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<string>>(json);
        }
    }
}

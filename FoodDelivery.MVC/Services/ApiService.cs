using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FoodDelivery.MVC.Models;

namespace FoodDelivery.MVC.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public ApiService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
            _httpClient.BaseAddress = new Uri(_config["ApiSettings:BaseUrl"]);
        }

        public async Task<string> LoginAsync(LoginViewModel login)
        {
            var content = new StringContent(JsonSerializer.Serialize(login), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonDocument.Parse(json).RootElement.GetProperty("token").GetString();
            }

            return null;
        }

        public async Task<HttpResponseMessage> PostAsync(string endpoint, object data)
        {
            

            var response = await _httpClient.PostAsJsonAsync(endpoint, data);
            return response;
        }
    }
}

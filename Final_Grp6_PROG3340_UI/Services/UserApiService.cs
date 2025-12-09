using Final_Grp6_PROG3340_UI.Models.ViewModels;
using System.Net.Http.Json;

namespace Final_Grp6_PROG3340_UI.Services
{
    public class UserApiService
    {
        private readonly ApiClient _apiClient;
        private readonly ILogger<UserApiService> _logger;

        public UserApiService(ApiClient apiClient, ILogger<UserApiService> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<List<UserViewModel>> GetAllUsersAsync()
        {
            try
            {
                var response = await _apiClient.GetAsync("users");

                if (response.IsSuccessStatusCode)
                {
                    var users = await response.Content.ReadFromJsonAsync<List<UserViewModel>>();
                    return users ?? new List<UserViewModel>();
                }

                _logger.LogWarning("Failed to get users. Status: {Status}", response.StatusCode);
                return new List<UserViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return new List<UserViewModel>();
            }
        }

        public async Task<UserViewModel?> GetUserByIdAsync(int id)
        {
            try
            {
                var response = await _apiClient.GetAsync($"users/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserViewModel>();
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user {UserId}", id);
                return null;
            }
        }
    }
}

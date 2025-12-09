using Final_Grp6_PROG3340_UI.Models.ViewModels;
using System.Net.Http.Json;

namespace Final_Grp6_PROG3340_UI.Services
{
    public class AuthApiService
    {
        private readonly ApiClient _apiClient;
        private readonly ILogger<AuthApiService> _logger;

        public AuthApiService(ApiClient apiClient, ILogger<AuthApiService> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<(bool Success, string? Token, string? ErrorMessage)> LoginAsync(string username, string password)
        {
            try
            {
                var response = await _apiClient.PostAsync("auth/login", new { username, password });

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    return (true, result?.Token, null);
                }

                var errorMessage = await response.Content.ReadAsStringAsync();
                return (false, null, errorMessage ?? "Login failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return (false, null, "An error occurred during login");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> RegisterAsync(RegisterViewModel model)
        {
            try
            {
                var response = await _apiClient.PostAsync("auth/register", new
                {
                    name = model.Name,
                    email = model.Email,
                    username = model.Username,
                    password = model.Password
                });

                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }

                var errorMessage = await response.Content.ReadAsStringAsync();
                return (false, errorMessage ?? "Registration failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return (false, "An error occurred during registration");
            }
        }

        public async Task<(bool Success, string? Token, string? ErrorMessage)> GoogleLoginAsync(string idToken)
        {
            try
            {
                var response = await _apiClient.PostAsync("auth/google", new { idToken });

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    return (true, result?.Token, null);
                }

                var errorMessage = await response.Content.ReadAsStringAsync();
                return (false, null, errorMessage ?? "Google login failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Google login");
                return (false, null, "An error occurred during Google login");
            }
        }

        public async Task<UserViewModel?> GetCurrentUserAsync()
        {
            try
            {
                var response = await _apiClient.GetAsync("auth/me");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserViewModel>();
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user");
                return null;
            }
        }

        private class LoginResponse
        {
            public string Token { get; set; } = string.Empty;
        }
    }
}

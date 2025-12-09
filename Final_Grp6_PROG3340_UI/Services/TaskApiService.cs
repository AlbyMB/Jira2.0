using Final_Grp6_PROG3340_UI.Models.ViewModels;
using System.Net.Http.Json;

namespace Final_Grp6_PROG3340_UI.Services
{
    public class TaskApiService
    {
        private readonly ApiClient _apiClient;
        private readonly ILogger<TaskApiService> _logger;

        public TaskApiService(ApiClient apiClient, ILogger<TaskApiService> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<List<TaskViewModel>> GetAllTasksAsync()
        {
            try
            {
                var response = await _apiClient.GetAsync("tasks");

                if (response.IsSuccessStatusCode)
                {
                    var tasks = await response.Content.ReadFromJsonAsync<List<TaskViewModel>>();
                    return tasks ?? new List<TaskViewModel>();
                }

                _logger.LogWarning("Failed to get tasks. Status: {Status}", response.StatusCode);
                return new List<TaskViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all tasks");
                return new List<TaskViewModel>();
            }
        }

        public async Task<TaskViewModel?> GetTaskByIdAsync(int id)
        {
            try
            {
                var response = await _apiClient.GetAsync($"tasks/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<TaskViewModel>();
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task {TaskId}", id);
                return null;
            }
        }

        public async Task<List<TaskViewModel>> GetMyTasksAsync()
        {
            try
            {
                var response = await _apiClient.GetAsync("tasks/my");

                if (response.IsSuccessStatusCode)
                {
                    var tasks = await response.Content.ReadFromJsonAsync<List<TaskViewModel>>();
                    return tasks ?? new List<TaskViewModel>();
                }

                return new List<TaskViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting my tasks");
                return new List<TaskViewModel>();
            }
        }

        public async Task<List<TaskViewModel>> GetAssignedTasksAsync()
        {
            try
            {
                var response = await _apiClient.GetAsync("tasks/assigned");

                if (response.IsSuccessStatusCode)
                {
                    var tasks = await response.Content.ReadFromJsonAsync<List<TaskViewModel>>();
                    return tasks ?? new List<TaskViewModel>();
                }

                return new List<TaskViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting assigned tasks");
                return new List<TaskViewModel>();
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateTaskAsync(TaskViewModel task)
        {
            try
            {
                var response = await _apiClient.PostAsync("tasks", new
                {
                    title = task.Title,
                    description = task.Description,
                    status = task.Status,
                    assignedToId = task.AssignedToId
                });

                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }

                var errorMessage = await response.Content.ReadAsStringAsync();
                return (false, errorMessage ?? "Failed to create task");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                return (false, "An error occurred while creating the task");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateTaskAsync(int id, TaskViewModel task)
        {
            try
            {
                var response = await _apiClient.PutAsync($"tasks/{id}", new
                {
                    title = task.Title,
                    description = task.Description,
                    status = task.Status,
                    assignedToId = task.AssignedToId
                });

                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }

                var errorMessage = await response.Content.ReadAsStringAsync();
                return (false, errorMessage ?? "Failed to update task");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task {TaskId}", id);
                return (false, "An error occurred while updating the task");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteTaskAsync(int id)
        {
            try
            {
                var response = await _apiClient.DeleteAsync($"tasks/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }

                var errorMessage = await response.Content.ReadAsStringAsync();
                return (false, errorMessage ?? "Failed to delete task");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task {TaskId}", id);
                return (false, "An error occurred while deleting the task");
            }
        }
    }
}

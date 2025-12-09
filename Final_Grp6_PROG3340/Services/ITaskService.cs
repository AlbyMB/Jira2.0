using Final_Grp6_PROG3340.Models;

namespace Final_Grp6_PROG3340.Services
{
	public interface ITaskService
	{
		Task<IEnumerable<TaskItem>> GetAllAsync();
		Task<TaskItem?> GetByIdAsync(int id);
		Task<IEnumerable<TaskItem>> GetByCreatorAsync(int creatorId);
		Task<IEnumerable<TaskItem>> GetByAssigneeAsync(int assigneeId);
		Task<TaskItem> CreateAsync(TaskItem task);
		Task<bool> UpdateAsync(TaskItem task);
		Task<bool> DeleteAsync(int id);
	}
}

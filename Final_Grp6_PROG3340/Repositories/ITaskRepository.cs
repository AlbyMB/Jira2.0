using Final_Grp6_PROG3340.Models;

namespace Final_Grp6_PROG3340.Repositories
{
	public interface ITaskRepository : IRepository<TaskItem>
	{
		Task<IEnumerable<TaskItem>> GetByCreatorAsync(int creatorId);
		Task<IEnumerable<TaskItem>> GetByAssigneeAsync(int assigneeId);
	}
}

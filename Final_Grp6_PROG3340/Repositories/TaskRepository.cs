using Final_Grp6_PROG3340.Data;
using Final_Grp6_PROG3340.Models;
using Microsoft.EntityFrameworkCore;

namespace Final_Grp6_PROG3340.Repositories
{
	public class TaskRepository : Repository<TaskItem>, ITaskRepository
	{
		public TaskRepository(AppDbContext context) : base(context) { }

		public async Task<IEnumerable<TaskItem>> GetByCreatorAsync(int creatorId)
		{
			return await _dbSet
				.Where(t => t.CreatedById == creatorId)
				.Include(t => t.CreatedBy)
				.Include(t => t.AssignedTo)
				.ToListAsync();
		}

		public async Task<IEnumerable<TaskItem>> GetByAssigneeAsync(int assigneeId)
		{
			return await _dbSet
				.Where(t => t.AssignedToId == assigneeId)
				.Include(t => t.CreatedBy)
				.Include(t => t.AssignedTo)
				.ToListAsync();
		}
	}
}

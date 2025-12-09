using Final_Grp6_PROG3340.Models;
using Final_Grp6_PROG3340.UOfW;

namespace Final_Grp6_PROG3340.Services
{
	public class TaskService : ITaskService
	{
		private readonly IUnitOfWork _uow;

		public TaskService(IUnitOfWork uow)
		{
			_uow = uow;
		}

		public async Task<IEnumerable<TaskItem>> GetAllAsync()
		{
			return await _uow.Tasks.GetAllAsync();
		}

		public async Task<TaskItem?> GetByIdAsync(int id)
		{
			return await _uow.Tasks.GetByIdAsync(id);
		}

		public async Task<IEnumerable<TaskItem>> GetByCreatorAsync(int creatorId)
		{
			return await _uow.Tasks.GetByCreatorAsync(creatorId);
		}

		public async Task<IEnumerable<TaskItem>> GetByAssigneeAsync(int assigneeId)
		{
			return await _uow.Tasks.GetByAssigneeAsync(assigneeId);
		}

		public async Task<TaskItem> CreateAsync(TaskItem task)
		{
			await _uow.Tasks.AddAsync(task);
			await _uow.SaveChangesAsync();
			return task;
		}

		public async Task<bool> UpdateAsync(TaskItem task)
		{
			_uow.Tasks.Update(task);
			return await _uow.SaveChangesAsync() > 0;
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var existing = await _uow.Tasks.GetByIdAsync(id);

			if (existing == null)
				return false;

			_uow.Tasks.Remove(existing);
			return await _uow.SaveChangesAsync() > 0;
		}
	}
}

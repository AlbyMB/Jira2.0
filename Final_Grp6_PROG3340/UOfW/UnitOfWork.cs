using Final_Grp6_PROG3340.Data;
using Final_Grp6_PROG3340.Repositories;

namespace Final_Grp6_PROG3340.UOfW
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly AppDbContext _context;

		public IUserRepository Users { get; }
		public ITaskRepository Tasks { get; }

		public UnitOfWork(AppDbContext context,
			IUserRepository userRepository,
			ITaskRepository taskRepository)
		{
			_context = context;
			Users = userRepository;
			Tasks = taskRepository;
		}

		public async Task<int> SaveChangesAsync()
		{
			return await _context.SaveChangesAsync();
		}
	}
}

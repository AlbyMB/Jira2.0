using Final_Grp6_PROG3340.Repositories;

namespace Final_Grp6_PROG3340.UOfW
{
	public interface IUnitOfWork
	{
		IUserRepository Users { get; }
		ITaskRepository Tasks { get; }
		Task<int> SaveChangesAsync();
	}
}

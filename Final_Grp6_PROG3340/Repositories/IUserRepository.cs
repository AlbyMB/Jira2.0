using Final_Grp6_PROG3340.Models;

namespace Final_Grp6_PROG3340.Repositories
{
	public interface IUserRepository : IRepository<User>
	{
		Task<User?> GetByUsernameAsync(string username);
	}
}

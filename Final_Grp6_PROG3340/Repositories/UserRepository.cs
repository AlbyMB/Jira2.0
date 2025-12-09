using Final_Grp6_PROG3340.Data;
using Final_Grp6_PROG3340.Models;
using Microsoft.EntityFrameworkCore;

namespace Final_Grp6_PROG3340.Repositories
{
	public class UserRepository : Repository<User>, IUserRepository
	{
		public UserRepository(AppDbContext context) : base(context) { }

		public async Task<User?> GetByUsernameAsync(string username)
		{
			return await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
		}
	}
}

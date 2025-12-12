using Final_Grp6_PROG3340.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Xml.Linq;

namespace Final_Grp6_PROG3340.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}

		public DbSet<User> Users { get; set; }
		public DbSet<TaskItem> Tasks { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<TaskItem>()
				.HasOne(t => t.CreatedBy)
				.WithMany(u => u.CreatedTasks)
				.HasForeignKey(t => t.CreatedById)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<TaskItem>()
				.HasOne(t => t.AssignedTo)
				.WithMany(u => u.AssignedTasks)
				.HasForeignKey(t => t.AssignedToId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}


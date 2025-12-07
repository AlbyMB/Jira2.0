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

			modelBuilder.Entity<User>().HasData(
				 new User
				 {
					 Id = 1,
					 Name = "Admin User",
					 Email = "admin@example.com",
					 Username = "admin",
					 PasswordHash = "admin123", // placeholder
					 Role = "Admin",
					 CreatedAt = DateTime.UtcNow
				 },
				 new User
				 {
					 Id = 2,
					 Name = "John Doe",
					 Email = "john@example.com",
					 Username = "john",
					 PasswordHash = "pass123",
					 Role = "User",
					 CreatedAt = DateTime.UtcNow
				 },
				 new User
				 {
					 Id = 3,
					 Name = "Sarah Lee",
					 Email = "sarah@example.com",
					 Username = "sarah",
					 PasswordHash = "pass123",
					 Role = "User",
					 CreatedAt = DateTime.UtcNow
				 },
				 new User
				 {
					 Id = 4,
					 Name = "Michael Chen",
					 Email = "michael@example.com",
					 Username = "michael",
					 PasswordHash = "pass123",
					 Role = "User",
					 CreatedAt = DateTime.UtcNow
				 },
				 new User
				 {
					 Id = 5,
					 Name = "Emily Stone",
					 Email = "emily@example.com",
					 Username = "emily",
					 PasswordHash = "pass123",
					 Role = "User",
					 CreatedAt = DateTime.UtcNow
				 }
			);

			modelBuilder.Entity<TaskItem>().HasData(
				new TaskItem
				{
					Id = 1,
					Title = "Initial Project Setup",
					Description = "Create solution + basic API structure",
					Status = Models.TaskStatus.ToDo,
					CreatedById = 1,       // admin
					AssignedToId = 2,      // john
					IsArchived = false,
					CreatedAt = DateTime.UtcNow,
					UpdatedAt = DateTime.UtcNow
				},
				new TaskItem
				{
					Id = 2,
					Title = "Design Database Models",
					Description = "User + Task models with relationships",
					Status = Models.TaskStatus.Development,
					CreatedById = 2,
					AssignedToId = 2,
					IsArchived = false,
					CreatedAt = DateTime.UtcNow.AddMinutes(-20),
					UpdatedAt = DateTime.UtcNow.AddMinutes(-10)
				},
				new TaskItem
				{
					Id = 3,
					Title = "Implement Task CRUD",
					Description = "Basic CRUD endpoints",
					Status = Models.TaskStatus.Review,
					CreatedById = 3,
					AssignedToId = 3,
					IsArchived = false,
					CreatedAt = DateTime.UtcNow.AddMinutes(-40),
					UpdatedAt = DateTime.UtcNow.AddMinutes(-5)
				},
				new TaskItem
				{
					Id = 4,
					Title = "Setup Swagger",
					Description = "Enable Swagger UI + documentation",
					Status = Models.TaskStatus.Merge,
					CreatedById = 1,
					AssignedToId = 4,
					IsArchived = false,
					CreatedAt = DateTime.UtcNow.AddHours(-1),
					UpdatedAt = DateTime.UtcNow.AddMinutes(-20)
				},
				new TaskItem
				{
					Id = 5,
					Title = "Frontend Wireframes",
					Description = "High-level UI mockups",
					Status = Models.TaskStatus.Done,
					CreatedById = 5,
					AssignedToId = 5,
					IsArchived = false,
					CreatedAt = DateTime.UtcNow.AddHours(-2),
					UpdatedAt = DateTime.UtcNow.AddMinutes(-30)
				}
			);
		}
	}
}


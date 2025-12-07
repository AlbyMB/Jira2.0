namespace Final_Grp6_PROG3340.Models
{
	public class User
	{
		public int Id { get; set; }

		public string Name { get; set; } = default!;

		public string Email { get; set; } = default!;

		public string Username { get; set; } = default!;

		public string PasswordHash { get; set; } = default!;

		public string Role { get; set; } = "User";

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		// Navigation
		public ICollection<TaskItem> CreatedTasks { get; set; } = new List<TaskItem>();
		public ICollection<TaskItem?> AssignedTasks { get; set; } = new List<TaskItem>();
	}
}

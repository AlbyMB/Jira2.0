namespace Final_Grp6_PROG3340.Models
{
	public enum TaskStatus
	{
		ToDo,
		Development,
		Review,
		Merge,
		Done
	}
	public class TaskItem
	{
		public int Id { get; set; }

        public string Title { get; set; } = default!;

        public string? Description { get; set; }

        public TaskStatus Status { get; set; } = TaskStatus.ToDo;

        public int CreatedById { get; set; }
        public User CreatedBy { get; set; } = default!;

        public int? AssignedToId { get; set; }
        public User? AssignedTo { get; set; }

        public bool IsArchived { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ArchivedAt { get; set; }
	}
}

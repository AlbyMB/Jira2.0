using System.ComponentModel.DataAnnotations;

namespace Final_Grp6_PROG3340_UI.Models.ViewModels
{
    public class TaskViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public TaskStatus Status { get; set; }

        public int? AssignedToId { get; set; }
        public string? AssignedToName { get; set; }

        public int? CreatedById { get; set; }
        public string? CreatedByName { get; set; }

        public bool IsArchived { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? ArchivedAt { get; set; }
    }

    public enum TaskStatus
    {
        ToDo,
        Development,
        Review,
        Merge,
        Done
    }
}

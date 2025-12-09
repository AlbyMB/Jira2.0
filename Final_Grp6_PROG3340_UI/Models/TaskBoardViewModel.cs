using Final_Grp6_PROG3340_UI.Models.ViewModels;

namespace Final_Grp6_PROG3340_UI.Models
{
    public class TaskBoardViewModel
    {
        public List<TaskViewModel> ToDoTasks { get; set; } = new();
        public List<TaskViewModel> DevelopmentTasks { get; set; } = new();
        public List<TaskViewModel> ReviewTasks { get; set; } = new();
        public List<TaskViewModel> MergeTasks { get; set; } = new();
        public List<TaskViewModel> DoneTasks { get; set; } = new();
    }
}

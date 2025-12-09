using Final_Grp6_PROG3340_UI.Models;
using Final_Grp6_PROG3340_UI.Models.ViewModels;
using Final_Grp6_PROG3340_UI.Services;
using Microsoft.AspNetCore.Mvc;
using TaskStatus = Final_Grp6_PROG3340_UI.Models.ViewModels.TaskStatus;

namespace Final_Grp6_PROG3340_UI.Controllers
{
    public class TasksController : Controller
    {
        private readonly TaskApiService _taskService;
        private readonly UserApiService _userService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(TaskApiService taskService, UserApiService userService, ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _userService = userService;
            _logger = logger;
        }

        // Task Board (Kanban View)
        [HttpGet]
        public async System.Threading.Tasks.Task<IActionResult> Board()
        {
            var tasks = await _taskService.GetAllTasksAsync();

            var boardModel = new TaskBoardViewModel
            {
                ToDoTasks = tasks.Where(t => t.Status == TaskStatus.ToDo && !t.IsArchived).ToList(),
                DevelopmentTasks = tasks.Where(t => t.Status == TaskStatus.Development && !t.IsArchived).ToList(),
                ReviewTasks = tasks.Where(t => t.Status == TaskStatus.Review && !t.IsArchived).ToList(),
                MergeTasks = tasks.Where(t => t.Status == TaskStatus.Merge && !t.IsArchived).ToList(),
                DoneTasks = tasks.Where(t => t.Status == TaskStatus.Done && !t.IsArchived).ToList()
            };

            return View(boardModel);
        }

        // Task Details
        [HttpGet]
        public async System.Threading.Tasks.Task<IActionResult> Details(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);

            if (task == null)
            {
                TempData["ErrorMessage"] = "Task not found";
                return RedirectToAction(nameof(Board));
            }

            return View(task);
        }

        // Create Task - GET
        [HttpGet]
        public async System.Threading.Tasks.Task<IActionResult> Create()
        {
            var users = await _userService.GetAllUsersAsync();
            ViewBag.Users = users;
            return View();
        }

        // Create Task - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<IActionResult> Create(TaskViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var users = await _userService.GetAllUsersAsync();
                ViewBag.Users = users;
                return View(model);
            }

            var (success, errorMessage) = await _taskService.CreateTaskAsync(model);

            if (success)
            {
                TempData["SuccessMessage"] = "Task created successfully!";
                return RedirectToAction(nameof(Board));
            }

            ModelState.AddModelError(string.Empty, errorMessage ?? "Failed to create task");
            var allUsers = await _userService.GetAllUsersAsync();
            ViewBag.Users = allUsers;
            return View(model);
        }

        // Edit Task - GET
        [HttpGet]
        public async System.Threading.Tasks.Task<IActionResult> Edit(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);

            if (task == null)
            {
                TempData["ErrorMessage"] = "Task not found";
                return RedirectToAction(nameof(Board));
            }

            var users = await _userService.GetAllUsersAsync();
            ViewBag.Users = users;
            return View(task);
        }

        // Edit Task - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<IActionResult> Edit(int id, TaskViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                var users = await _userService.GetAllUsersAsync();
                ViewBag.Users = users;
                return View(model);
            }

            var (success, errorMessage) = await _taskService.UpdateTaskAsync(id, model);

            if (success)
            {
                TempData["SuccessMessage"] = "Task updated successfully!";
                return RedirectToAction(nameof(Details), new { id });
            }

            ModelState.AddModelError(string.Empty, errorMessage ?? "Failed to update task");
            var allUsers = await _userService.GetAllUsersAsync();
            ViewBag.Users = allUsers;
            return View(model);
        }

        // Delete Task
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<IActionResult> Delete(int id)
        {
            var (success, errorMessage) = await _taskService.DeleteTaskAsync(id);

            if (success)
            {
                TempData["SuccessMessage"] = "Task deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = errorMessage ?? "Failed to delete task";
            }

            return RedirectToAction(nameof(Board));
        }

        // My Tasks
        [HttpGet]
        public async System.Threading.Tasks.Task<IActionResult> MyTasks()
        {
            var tasks = await _taskService.GetMyTasksAsync();
            return View(tasks);
        }

        // Assigned Tasks
        [HttpGet]
        public async System.Threading.Tasks.Task<IActionResult> AssignedTasks()
        {
            var tasks = await _taskService.GetAssignedTasksAsync();
            return View(tasks);
        }
    }
}

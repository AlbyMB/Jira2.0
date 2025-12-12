using Final_Grp6_PROG3340.Models;
using Final_Grp6_PROG3340.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Final_Grp6_PROG3340.Hubs;

namespace Final_Grp6_PROG3340.Controllers
{
	[ApiController]
	[Route("api/tasks")]
	public class TaskController : ControllerBase
	{
		private readonly ITaskService _taskService;
		private readonly IMemoryCache _cache;
		private readonly IHubContext<TaskHub> _hub;

		public TaskController(
			ITaskService taskService,
			IMemoryCache memoryCache,
			IHubContext<TaskHub> hub)
		{
			_taskService = taskService;
			_cache = memoryCache;
			_hub = hub;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllTasks()
		{
			if (!_cache.TryGetValue("all_tasks", out List<TaskItem>? tasks))
			{
				Response.Headers.Append("X-Cache", "MISS");
				await Task.Delay(2000);
				tasks = (List<TaskItem>?)await _taskService.GetAllAsync();

				if (tasks is null)
					return NotFound();

				_cache.Set("all_tasks", tasks,
					new MemoryCacheEntryOptions
					{
						SlidingExpiration = TimeSpan.FromMinutes(5)
					});

				return Ok(tasks);
			}

			Response.Headers.Append("X-Cache", "HIT");
			return Ok(tasks!);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetTaskById(int id)
		{
			var cacheKey = $"task_{id}";

			if (!_cache.TryGetValue(cacheKey, out TaskItem? task))
			{
				Response.Headers.Append("X-Cache", "MISS");
				await Task.Delay(2000);

				task = await _taskService.GetByIdAsync(id);

				if (task is null)
					return NotFound();

				_cache.Set(cacheKey, task,
					new MemoryCacheEntryOptions
					{
						SlidingExpiration = TimeSpan.FromMinutes(5)
					});

				return Ok(task);
			}

			Response.Headers.Append("X-Cache", "HIT");
			return Ok(task!);
		}

		[HttpPost]
		public async Task<IActionResult> CreateTask([FromBody] TaskItem task)
		{
			var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (!int.TryParse(userIdStr, out var userId))
				return Unauthorized("User ID not found in token");
			task.CreatedById = userId;
			var created = await _taskService.CreateAsync(task);

			// Notify all users
			await _hub.Clients.All.SendAsync("TaskCreated", $"Task created: {created.Title}");

			if (task.AssignedToId != null)
			{
				await _hub.Clients.User(task.AssignedToId.ToString())
					.SendAsync("TaskAssigned", $"You have been assigned to: {task.Title}");
			}

			return Ok(created);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskItem task)
		{
			_cache.Remove($"task_{id}");

			var updated = await _taskService.UpdateAsync(task);

			// Notify task creator only
			if (task.CreatedById != 0)
			{
				await _hub.Clients.User(task.CreatedById.ToString())
					.SendAsync("TaskStatusUpdated", $"Task '{task.Title}' status updated to: {task.Status}");
			}

			return Ok(updated);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTask(int id)
		{
			// Get task before delete so we can broadcast title
			var existing = await _taskService.GetByIdAsync(id);

			await _taskService.DeleteAsync(id);

			_cache.Remove($"task_{id}");

			if (existing != null)
			{
				// Notify all users
				await _hub.Clients.All.SendAsync("TaskDeleted", $"Task deleted: {existing.Title}");
			}

			return NoContent();
		}

		[HttpGet("my")]
		public async Task<IActionResult> GetMyTasks()
		{
			var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (!int.TryParse(userIdStr, out var userId))
				return Unauthorized("User ID not found in token");

			return Ok(await _taskService.GetByCreatorAsync(userId));
		}

		[HttpGet("assigned")]
		public async Task<IActionResult> GetAssignedTasks()
		{
			var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (!int.TryParse(userIdStr, out var userId))
				return Unauthorized("User ID not found in token");

			return Ok(await _taskService.GetByAssigneeAsync(userId));
		}
	}
}

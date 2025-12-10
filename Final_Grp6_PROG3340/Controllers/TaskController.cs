using Final_Grp6_PROG3340.Models;
using Final_Grp6_PROG3340.Repositories;
using Final_Grp6_PROG3340.Services;
using Final_Grp6_PROG3340.UOfW;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Final_Grp6_PROG3340.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        private readonly IMemoryCache _cache; // in-Memory caching

        // TODO: inject user service
        public TaskController(ITaskService taskService, IMemoryCache memoryCache)
        {
            _taskService = taskService;
            _cache = memoryCache;
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
                {
                    return NotFound();
                }
                _cache.Set("all_tasks", tasks, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(5)
                });
                return Ok(tasks);
            }
            else
            {
                Response.Headers.Append("X-Cache", "HIT");
                if (tasks is null)
                {
                    return NotFound();
                }
                return Ok(tasks);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            string cacheKey = $"task_{id}";
            if(!_cache.TryGetValue(cacheKey, out TaskItem? task))
            {
                Response.Headers.Append("X-Cache", "MISS");
                await Task.Delay(2000);
                task = await _taskService.GetByIdAsync(id);
                if (task is null)
                {
                    return NotFound();
                }
                _cache.Set(cacheKey, task, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(5)
                });
                return Ok(task);
            }
            else
            {
                Response.Headers.Append("X-Cache", "HIT");
                if (task is null)
                {
                    return NotFound();
                }
                return Ok(task);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TaskItem task)
        {
            return Ok(await _taskService.CreateAsync(task));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskItem task)
        {
            _cache.Remove($"task_{id}");
            return Ok(await _taskService.UpdateAsync(task));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            await _taskService.DeleteAsync(id);
            _cache.Remove($"task_{id}");
            return NoContent();
        }

        // TODO: implement id
        [HttpGet("my")]
        public async Task<IActionResult> GetMyTasks()
        {
            return Ok(await _taskService.GetByCreatorAsync(0));
        }

        // TODO: implement id
        [HttpGet("assigned")]
        public async Task<IActionResult> GetAssignedTasks()
        {
            return Ok(await _taskService.GetByAssigneeAsync(0));
        }
    }
}

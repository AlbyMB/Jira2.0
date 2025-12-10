using Final_Grp6_PROG3340.Models;
using Final_Grp6_PROG3340.Repositories;
using Final_Grp6_PROG3340.Services;
using Final_Grp6_PROG3340.UOfW;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Final_Grp6_PROG3340.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            return Ok(await _taskService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            const string cacheKey = $"task_{id}"
            if(!_cache.TryGetValue(cacheKey, out Task<IActionResult> task))
            {
                Response.Headers.Add("X-Cache", "MISS");
                // delay to simulate slow database call
                await Task.Delay(2000); // 2 seconds delay
                if (await _taskService.GetByIdAsync(id) is not TaskItem task)
                {
                    return NotFound();
                }
                // Cache for 5 minutes
                _cache.Set(cacheKey, task, TimeSpan.FromMinutes(5));
            }
            else
            {
                Response.Headers.Add("X-Cache", "HIT");
            }
            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TaskItem task)
        {
            return Ok(await _taskService.CreateAsync(task));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskItem task)
        {
            //if (_cache.TryGetValue($"task_{id}")) // check the cache for this task id
            //{
            // this task exists in cache, so remove it
            _cache.Remove($"task_{id}");
            //}
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

        // In-memory caching
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Try to get products from cache
            if (!_cache.TryGetValue("products", out List<Product> products))
            {
                // Simulate expensive DB operation
                await Task.Delay(2000); // 2 seconds delay
                products = await _context.Products.ToListAsync();
                _cache.Set("products", products, TimeSpan.FromMinutes(5));
            }
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Invalidate cache
            _cache.Remove("products"); // need to remove cache so that data is up-to-date
            return CreatedAtAction(nameof(GetAll), new { id = product.Id }, product);
        }
    }
}

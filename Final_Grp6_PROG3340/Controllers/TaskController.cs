using Final_Grp6_PROG3340.Models;
using Final_Grp6_PROG3340.Repositories;
using Final_Grp6_PROG3340.Services;
using Final_Grp6_PROG3340.UOfW;
using Microsoft.AspNetCore.Mvc;

namespace Final_Grp6_PROG3340.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        // TODO: inject user service
        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            return Ok(await _taskService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            if(await _taskService.GetByIdAsync(id) is not TaskItem task)
            {
                return NotFound();
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
            return Ok(await _taskService.UpdateAsync(task));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            await _taskService.DeleteAsync(id);
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

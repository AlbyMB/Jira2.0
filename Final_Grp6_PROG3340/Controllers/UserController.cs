using Final_Grp6_PROG3340.Models;
using Final_Grp6_PROG3340.UOfW;
using Microsoft.AspNetCore.Mvc;

namespace Final_Grp6_PROG3340.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await _unitOfWork.Users.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            if (await _unitOfWork.Users.GetByIdAsync(id) is not User user)
            {
                return NotFound();
            }
            return Ok(user);
        }
    }
}

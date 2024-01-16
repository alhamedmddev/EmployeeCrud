using EmployeeCrud.Server.Iservices;
using EmployeeCrud.Shared.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeCrud.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> UserRegistration(UserVM userVM)
        {
            var result = await _userService.RegisterNewUserAsync(userVM);
            if (result.IsUserRegistered)
            {
                return Ok(result.Message);
            }
            ModelState.AddModelError("Email", result.Message);
            return BadRequest(ModelState);
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginVM payload)
        {
            var result = await _userService.LoginAsync(payload);
            if (result.IsLoginSuccess)
            {
                return Ok(result.TokeResponse);
            }
            ModelState.AddModelError("LoginError", "Invalid Credentials");
            return BadRequest(ModelState);
        }
    }
}

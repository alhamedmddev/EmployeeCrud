using EmployeeCrud.Client.Pages;
using EmployeeCrud.Server.Iservices;
using EmployeeCrud.Server.Services;
using EmployeeCrud.Shared.Model;
using EmployeeCrud.Shared.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeCrud.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
       
        [HttpGet("GetEmployees")]
       // [Authorize]
        public async Task<List<EmployeeVM>> GetEmployees()
        {
            var result = await _employeeService.GetEmployeeAsync();
           
            return result.employeeVM;
          
        }
        [HttpGet("GetEmployeeID/{id}")]
        public async Task<IActionResult> GetEmployeeID(int id)
        {
            var result = await _employeeService.GetEmployeeByIDAsync(id);
            if (result.IsGetSuccess)
            {
                return Ok(result.employeeVM);
            }
            return NotFound();
        }
        [HttpPost("SaveEmployee")]
        public async Task<IActionResult> SaveEmployee([FromBody] EmployeeVM employeeVM)
        {
            var result = await _employeeService.SaveNewEmpAsync(employeeVM);
            if (result.IsEmpSaved)
            {
                return Ok(result.message);
            }
            ModelState.AddModelError("Email", result.message);
            return BadRequest(ModelState);
        }
        [HttpPut("UpdateEmployee")]
        public async Task<IActionResult> UpdateEmployee([FromBody] EmployeeVM employeeVM)
        {
            var result = await _employeeService.UpdateEmpAsync(employeeVM);
            if (result.IsEmpSaved)
            {
                return Ok(result.message);
            }
            ModelState.AddModelError("Email", result.message);
            return BadRequest(ModelState);
        }
        [HttpDelete("DeleteEmployee/{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var result = await _employeeService.DeleteEMployeeAsync(id);
            if (result.IsGetSuccess)
            {
                return Ok(result.message);
            }
            ModelState.AddModelError("Email", result.message);
            return BadRequest(ModelState);
        }
    }
}

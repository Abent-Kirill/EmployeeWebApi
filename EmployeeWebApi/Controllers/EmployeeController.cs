using EmployeeWebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class EmployeeController(EmployeeRepository employeeRepository) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddEmployee([FromBody] Employee employee)
    {
        var id = await employeeRepository.AddEmployee(employee);
        return Ok(new { Id = id });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        await employeeRepository.DeleteEmployee(id);
        return NoContent();
    }

    [HttpGet("company/{companyId}")]
    public async Task<IActionResult> GetEmployeesByCompanyId(int companyId)
    {
        var employees = await employeeRepository.GetEmployeesByCompanyId(companyId);
        return Ok(employees);
    }

    [HttpGet("department/{departmentName}")]
    public async Task<IActionResult> GetEmployeesByDepartmentName(string departmentName)
    {
        var employees = await employeeRepository.GetEmployeesByDepartmentName(departmentName);
        return Ok(employees);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, [FromBody] Employee employee)
    {
        await employeeRepository.UpdateEmployee(id, employee);
        return NoContent();
    }
}

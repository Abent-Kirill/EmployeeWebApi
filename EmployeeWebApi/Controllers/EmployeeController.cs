using EmployeeWebApi.Models;

using Microsoft.AspNetCore.Mvc;

namespace EmployeeWebApi.Controllers;

[ApiController]
[Produces("application/json")]
[Route("[controller]")]
public sealed class EmployeeController(EmployeeRepository employeeRepository) : ControllerBase
{
    /// <summary>
    /// Добавление сотрудника
    /// </summary>
    /// <param name="employee">Данные сотрудника</param>
    /// <returns>Id добавленного сотрудника</returns>
    [HttpPost]
    public async Task<IActionResult> AddEmployee([FromBody] Employee employee)
    {
        var id = await employeeRepository.AddEmployee(employee);
        return Ok(new { Id = id });
    }

    /// <summary>
    /// Удаление сотрудника
    /// </summary>
    /// <param name="id">id сотрудника</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        await employeeRepository.DeleteEmployee(id);
        return NoContent();
    }

    /// <summary>
    /// Получить список всех доступных полей сотрудников для указанной компании
    /// </summary>
    /// <param name="companyId">Id компании</param>
    /// <returns>Список сотрудников</returns>
    [HttpGet("Company/{companyId}")]
    public async Task<IActionResult> GetEmployeesByCompanyId(int companyId)
    {
        var employees = await employeeRepository.GetEmployeesByCompanyId(companyId);
        return Ok(employees);
    }

    /// <summary>
    /// Получить список всех доступных полей сотрудников для указного отдела
    /// </summary>
    /// <param name="departmentName">Название отдела</param>
    /// <returns>Список сотрудников</returns>
    [HttpGet("Department/{departmentName}")]
    public async Task<IActionResult> GetEmployeesByDepartmentName(string departmentName)
    {
        var employees = await employeeRepository.GetEmployeesByDepartmentName(departmentName);
        return Ok(employees);
    }

    /// <summary>
    /// Изменение сотрудника по его Id. Изменения должно быть только тех полей, которые указаны в запросе.
    /// </summary>
    /// <param name="id">id сотрудника</param>
    /// <param name="employee">Обновленные данные</param>
    /// <returns></returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateEmployee(int id, [FromBody] Employee employee)
    {
        try
        {
            await employeeRepository.UpdateEmployee(id, employee);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { ex.Message });
        }
    }
}

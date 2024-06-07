using System.ComponentModel.DataAnnotations;

namespace EmployeeWebApi.Models;

public sealed class Department
{
    public int DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    [Phone]
    public string? Phone { get; set; }
}
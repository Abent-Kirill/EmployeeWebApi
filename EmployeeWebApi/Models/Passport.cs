using Microsoft.AspNetCore.Identity;

using System.ComponentModel.DataAnnotations;

namespace EmployeeWebApi.Models;

public sealed class Passport
{
    public int PassportId { get; set; }
    public string? Type { get; set; }
    [ProtectedPersonalData]
    [StringLength(10, MinimumLength = 10)]
    public string? Number { get; set; }
}
namespace EmployeeWebApi.Models;

public sealed class Passport
{
    public int PassportId { get; set; }
    public string? Type { get; set; }
    public string? Number { get; set; }
}
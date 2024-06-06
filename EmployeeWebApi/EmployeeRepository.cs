using Dapper;

using EmployeeWebApi.Models;
using System.Data;

namespace EmployeeWebApi;

public sealed class EmployeeRepository(IDbConnection dbConnection)
{
    public async Task<int> AddEmployee(Employee employee)
    {
        dbConnection.Open();
        using var transaction = dbConnection.BeginTransaction();
        int passportId = 0;
        if (employee.Passport != null)
        {
            var passportQuery = @"
            INSERT INTO Passports (Type, Number)
            VALUES (@Type, @Number);
            SELECT last_insert_rowid();";
            passportId = await dbConnection.QuerySingleAsync<int>(passportQuery, new
            {
                employee.Passport.Type,
                employee.Passport.Number
            }, transaction);
        }

        int departmentId = 0;
        if (employee.Department != null)
        {
            var departmentQuery = @"
            INSERT INTO Departments (Name, Phone)
            VALUES (@Name, @Phone);
            SELECT last_insert_rowid();";
            departmentId = await dbConnection.QuerySingleAsync<int>(departmentQuery, new
            {
                employee.Department.DepartmentName,
                employee.Department.Phone
            }, transaction);
        }

        var employeeQuery = @"
        INSERT INTO Employees (Name, Surname, Phone, CompanyId, PassportId, DepartmentId)
        VALUES (@Name, @Surname, @Phone, @CompanyId, @PassportId, @DepartmentId);
        SELECT last_insert_rowid();";

        var employeeId = await dbConnection.QuerySingleAsync<int>(employeeQuery, new
        {
            employee.Name,
            employee.Surname,
            employee.Phone,
            employee.CompanyId,
            PassportId = passportId != 0 ? passportId : (int?)null,
            DepartmentId = departmentId != 0 ? departmentId : (int?)null
        }, transaction);

        transaction.Commit();
        dbConnection.Close();
        return employeeId;
    }


    public async Task DeleteEmployee(int id)
    {
        dbConnection.Open();
        using var transaction = dbConnection.BeginTransaction();
        var employee = await dbConnection.QuerySingleOrDefaultAsync<Employee>("SELECT * FROM Employees WHERE Id = @Id", new { Id = id }, transaction);

        if (employee != null)
        {
            if (employee.Passport?.PassportId != null)
            {
                await dbConnection.ExecuteAsync("DELETE FROM Passports WHERE Id = @PassportId", new { employee.Passport.PassportId }, transaction);
            }

            if (employee.Department?.DepartmentId != null)
            {
                await dbConnection.ExecuteAsync("DELETE FROM Departments WHERE Id = @DepartmentId", new { employee.Department.DepartmentId }, transaction);
            }

            await dbConnection.ExecuteAsync("DELETE FROM Employees WHERE Id = @Id", new { Id = id }, transaction);
        }

        transaction.Commit();
        dbConnection.Close();
    }

    public async Task<IEnumerable<Employee>> GetEmployeesByCompanyId(int companyId)
    {
        var query = @"
    SELECT 
    e.*, 
    COALESCE(p.Id, 0) as PassportId, 
    p.Type as Type, 
    p.Number as Number, 
    COALESCE(d.Id, 0) as DepartmentId, 
    d.Name as DepartmentName, 
    d.Phone as Phone 
FROM 
    Employees e
LEFT JOIN 
    Passports p ON e.PassportId = p.Id
LEFT JOIN 
    Departments d ON e.DepartmentId = d.Id
WHERE e.CompanyId = @CompanyId";

        var employeeDictionary = new Dictionary<int, Employee>();

        var employees = await dbConnection.QueryAsync<Employee, Passport, Department, Employee>(
            query,
            (employee, passport, department) =>
            {
                if (!employeeDictionary.TryGetValue(employee.Id, out var employeeEntry))
                {
                    employeeEntry = employee;
                    employeeEntry.Passport = passport;
                    employeeEntry.Department = department;
                    employeeDictionary.Add(employee.Id, employeeEntry);
                }

                return employeeEntry;
            },
            splitOn: "PassportId,DepartmentId",
            param: new { CompanyId = companyId }
        );

        return employees.Distinct();
    }

    public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentName(string departmentName)
    {
        var query = @"
    SELECT 
    e.*, 
    COALESCE(p.Id, 0) as PassportId, 
    p.Type as Type, 
    p.Number as Number, 
    COALESCE(d.Id, 0) as DepartmentId, 
    d.Name as DepartmentName, 
    d.Phone as Phone 
FROM 
    Employees e
LEFT JOIN 
    Passports p ON e.PassportId = p.Id
LEFT JOIN 
    Departments d ON e.DepartmentId = d.Id
WHERE 
    d.Name = @DepartmentName
";

        var employeeDictionary = new Dictionary<int, Employee>();

        var employees = await dbConnection.QueryAsync<Employee, Passport, Department, Employee>(
    query,
    (employee, passport, department) =>
    {
        if (!employeeDictionary.TryGetValue(employee.Id, out var employeeEntry))
        {
            employeeEntry = employee;
        }
            employeeEntry.Passport = passport;
        employeeEntry.Department = department;

        employeeDictionary.Add(employee.Id, employeeEntry);
        return employeeEntry;
    },
    splitOn: "PassportId,DepartmentId",
    param: new { DepartmentName = departmentName }
);

        return employees.Distinct();
    }


    public async Task UpdateEmployee(int id, Employee employee)
    {
        dbConnection.Open();
        using var transaction = dbConnection.BeginTransaction();
        var updateFields = new List<string>();
        var parameters = new DynamicParameters();
        parameters.Add("Id", id);

        if (!string.IsNullOrEmpty(employee.Name))
        {
            updateFields.Add("Name = @Name");
            parameters.Add("Name", employee.Name);
        }

        if (!string.IsNullOrEmpty(employee.Surname))
        {
            updateFields.Add("Surname = @Surname");
            parameters.Add("Surname", employee.Surname);
        }

        if (!string.IsNullOrEmpty(employee.Phone))
        {
            updateFields.Add("Phone = @Phone");
            parameters.Add("Phone", employee.Phone);
        }

        if (employee.CompanyId != 0)
        {
            updateFields.Add("CompanyId = @CompanyId");
            parameters.Add("CompanyId", employee.CompanyId);
        }

        if (updateFields.Count > 0)
        {
            var updateQuery = $"UPDATE Employees SET {string.Join(", ", updateFields)} WHERE Id = @Id";
            await dbConnection.ExecuteAsync(updateQuery, parameters, transaction);
        }

        if (employee.Passport != null)
        {
            var passportQuery = @"
            INSERT INTO Passports (Id, Type, Number)
            VALUES (@Id, @Type, @Number)
            ON CONFLICT(Id) DO UPDATE SET
                Type = excluded.Type,
                Number = excluded.Number";
            await dbConnection.ExecuteAsync(passportQuery, new
            {
                employee.Passport.PassportId,
                employee.Passport.Type,
                employee.Passport.Number
            }, transaction);
        }

        if (employee.Department != null)
        {
            var departmentQuery = @"
            INSERT INTO Departments (Id, Name, Phone)
            VALUES (@Id, @Name, @Phone)
            ON CONFLICT(Id) DO UPDATE SET
                Name = excluded.Name,
                Phone = excluded.Phone";
            await dbConnection.ExecuteAsync(departmentQuery, new
            {
                employee.Department.DepartmentId,
                employee.Department.DepartmentName,
                employee.Department.Phone
            }, transaction);
        }

        transaction.Commit();
        dbConnection.Close();
    }

}

using Dapper;
using Microsoft.Data.Sqlite;
using System.Data;

namespace EmployeeWebApi.Data;

internal sealed class DatabaseContext : IDatabaseContext
{
    private readonly string _connectionString;

    public DatabaseContext(string connectionString)
    {
        _connectionString = connectionString;
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using IDbConnection db = new SqliteConnection(_connectionString);
        var employeeTableQuery = @"
        CREATE TABLE IF NOT EXISTS Employees (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL,
            Surname TEXT NOT NULL,
            Phone TEXT NOT NULL,
            CompanyId INTEGER NOT NULL,
            PassportId INTEGER,
            DepartmentId INTEGER,
            FOREIGN KEY (PassportId) REFERENCES Passports(Id),
            FOREIGN KEY (DepartmentId) REFERENCES Departments(Id)
        )";

        var passportTableQuery = @"
        CREATE TABLE IF NOT EXISTS Passports (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Type TEXT,
            Number TEXT
        )";

        var departmentTableQuery = @"
        CREATE TABLE IF NOT EXISTS Departments (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT,
            Phone TEXT
        )";

        db.Execute(employeeTableQuery);
        db.Execute(passportTableQuery);
        db.Execute(departmentTableQuery);
    }

    public IDbConnection CreateConnection()
    {
        return new SqliteConnection(_connectionString);
    }
}

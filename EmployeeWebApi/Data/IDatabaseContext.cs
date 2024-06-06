using System.Data;

namespace EmployeeWebApi.Data;

public interface IDatabaseContext
{
    IDbConnection CreateConnection();
}
# ТЗ
Web-Сервис сотрудников, сделанный на платформе .Net Core.

Сервис должен уметь:
- Добавлять сотрудников, в ответ должен приходить Id добавленного сотрудника;
- Удалять сотрудников по Id;
- Выводить список сотрудников для указанной компании. Все доступные поля;
- Выводить список сотрудников для указанного отдела компании. Все доступные поля;
- Изменять сотрудника по его Id. Изменения должно быть только тех полей, которые указаны в запросе.

Модель сотрудника:

``` json
{
  Id int
  Name string
  Surname string
  Phone string
  CompanyId int
  Passport {
    Type string
    Number string
  }
  Department {
    Name string
    Phone string
  }
}
```

Все методы должны быть реализованы в виде HTTP запросов в формате JSON.

БД: любая;

ORM: Dapper.

В качестве БД было использовано SQL Lite.

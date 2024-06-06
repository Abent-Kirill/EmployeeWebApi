using EmployeeWebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(new DatabaseContext(builder.Configuration.GetConnectionString("DefaultConnection")!));
builder.Services.AddScoped<EmployeeRepository>(
    serviceProvider =>
    {
        var dbContext = serviceProvider.GetRequiredService<DatabaseContext>();
        return new EmployeeRepository(dbContext.CreateConnection());
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

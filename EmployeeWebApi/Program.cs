using EmployeeWebApi;
using EmployeeWebApi.Data;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});
builder.Services.AddSingleton<IDatabaseContext, DatabaseContext>(x => 
new DatabaseContext(builder.Configuration.GetConnectionString("DefaultConnection")!));

builder.Services.AddScoped(
    serviceProvider =>
    {
        var dbContext = serviceProvider.GetRequiredService<IDatabaseContext>();
        return new EmployeeRepository(dbContext.CreateConnection());
    });

var app = builder.Build();
app.UseExceptionHandler("/error");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
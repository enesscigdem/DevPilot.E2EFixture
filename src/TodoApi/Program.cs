using TodoApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<ITodoAuditLogger, TodoAuditLogger>();
builder.Services.AddSingleton<ITodoService, TodoService>();

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();

public partial class Program { }

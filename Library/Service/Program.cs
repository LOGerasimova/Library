using Library.Application;
using Library.Infrastructure;
using Library.Service;

using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Слои приложения
builder.Services.AddService();
builder.Services.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Library API",
        Version = "v1",
        Description = "API для управления библиотекой."
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Library API v1");
    options.RoutePrefix = "swagger";
});

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Books}/{action=Index}/{id?}");
app.MapControllers();

app.Run();

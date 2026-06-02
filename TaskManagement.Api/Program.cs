using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Services;
using Project.Domain.Interfaces; 
using TaskManagement.Infrastructure.Persistence;
using TaskManagement.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. Base de Datos (In-Memory para el ejemplo)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("TasksDb"));

// 2. Inyección de Dependencias
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Agregar configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy => policy.WithOrigins("http://localhost:4200")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();

// Habilitar CORS usando la política AllowAngular
app.UseCors("AllowAngular");

// CORRECCIÓN: Forzamos el uso de Swagger en cualquier entorno para pruebas locales
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskManagement API V1");
    c.RoutePrefix = string.Empty; // <-- Esto hace que Swagger sea la página de inicio por defecto
});

// app.UseHttpsRedirection();

app.MapControllers();
app.Run();
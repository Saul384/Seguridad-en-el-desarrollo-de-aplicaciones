using Microsoft.EntityFrameworkCore;
using Serilog;
using VulnerableApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) // Lee el nivel mínimo y otros ajustes desde appsettings.json
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Seq("http://localhost:5341")
    .Enrich.FromLogContext()
    .Enrich.WithMachineName() // Enriquecedor adicional
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Registrar el DbContext usando la cadena de conexión de appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSession();

var app = builder.Build();

// Agregar Serilog Request Logging (opcional, pero muy útil para registrar peticiones HTTP)
// app.UseSerilogRequestLogging(); // <-- Lo comentamos porque ahora usamos GlobalLoggingMiddleware

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Integrar nuestro middleware de logging global
app.UseMiddleware<VulnerableApp.Middlewares.GlobalLoggingMiddleware>();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
    

// --- HACK PARA IMPRIMIR LA TABLA EN CONSOLA ---
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<VulnerableApp.Data.AppDbContext>();

    Console.WriteLine("\n=======================================================");
    Console.WriteLine(" EVIDENCIA 3: DATOS EN TEXTO PLANO (VulnerableDb)");
    Console.WriteLine("=======================================================");

    // Aplicar migraciones pendientes automáticamente para evitar el error "Invalid object name 'Users'"
    await context.Database.MigrateAsync();

    // Agregamos await y cambiamos a ToListAsync()
    foreach (var u in await context.Users.ToListAsync())
    {
        Console.WriteLine($" ID: {u.Id} | Usuario: {u.Username} | PasswordHash: {u.PasswordHash}");
    }

    Console.WriteLine("=======================================================\n");
}

// Agregamos await y cambiamos a RunAsync()
await app.RunAsync();



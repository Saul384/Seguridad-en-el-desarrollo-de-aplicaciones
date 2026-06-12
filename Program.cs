using Microsoft.EntityFrameworkCore;
using VulnerableApp.Data;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Registrar el DbContext usando la cadena de conexión de appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
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

    // Agregamos await y cambiamos a ToListAsync()
    foreach (var u in await context.Users.ToListAsync())
    {
        Console.WriteLine($" ID: {u.Id} | Usuario: {u.Username} | PasswordHash: {u.PasswordHash}");
    }

    Console.WriteLine("=======================================================\n");
}

// Agregamos await y cambiamos a RunAsync()
await app.RunAsync();



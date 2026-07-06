using Microsoft.EntityFrameworkCore;
using System;
using VulnerableApp.Models;

namespace VulnerableApp.Data
 // (Asegúrate de mantener tu propio namespace)
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Agregamos horas, minutos, segundos (0, 0, 0) y especificamos que es UTC
            var fechaEstatica = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", PasswordHash = "$2a$11$wZf6qk9ODzD6jpXbs9ljhuplDCeIPd7wGxuvdBIpO7HvYccdpVUjq", Email = "admin@test.com", Balance = 1000m, CreatedAt = fechaEstatica },
                new User { Id = 2, Username = "user1", PasswordHash = "$2a$11$YhH73NAPzWhdxKa8gmBn4Oqybk8HpkL4OFMgR9T0.U7mIzW6BnlV.", Email = "user@test.com", Balance = 500m, CreatedAt = fechaEstatica },
                new User { Id = 3, Username = "user2", PasswordHash = "$2a$11$oV8lldHlBORG8Fyh/c8Kl.WOhHJ5ys9ipQr2NPvLScq0gdrU4gQV2", Email = "user2@test.com", Balance = 750m, CreatedAt = fechaEstatica }
            );
        }
    }
}

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VulnerableApp.Models
{
    public class User
    {
        public int Id { get; set; }
        
        // Inicializamos con un texto vacío para evitar la advertencia de nulos
        public string Username { get; set; } = string.Empty;
        
        [Column("Password")] // Mapea a la BD para evitar errores
        public string PasswordHash { get; set; } = string.Empty;
        
        public string Email { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }
        
        public DateTime CreatedAt { get; set; }
    }
}
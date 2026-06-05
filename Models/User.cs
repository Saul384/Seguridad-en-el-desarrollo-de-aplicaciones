using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VulnerableApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        
        [Column("Password")] // Mapea a la BD para evitar errores
        public string PasswordHash { get; set; }
        
        public string Email { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }
        
        public DateTime CreatedAt { get; set; }
    }
}
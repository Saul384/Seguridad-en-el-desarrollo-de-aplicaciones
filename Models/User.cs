namespace VulnerableApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        
        // Vulnerable: sin hash
        public string Password { get; set; }
        
        public string Email { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
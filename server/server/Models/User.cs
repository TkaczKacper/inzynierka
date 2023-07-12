namespace server.Models
{
     public class User
     {
          public int ID { get; set; }
          public string Username { get; set; } = null!;
          public string Password { get; set; } = null!;
          public string? Email { get; set; }
          public DateTime RegisterDate { get; set; }
     }
}
namespace server.Models
{
     public class User
     {
        public int ID { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public DateTime RegisterDate { get; set; }
     }
}
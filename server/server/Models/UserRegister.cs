using System.ComponentModel.DataAnnotations;

namespace server.Models
{
     public class UserRegister
     {
          [StringLength(32, MinimumLength = 6, ErrorMessage = "Username too short.")]
          [Required(ErrorMessage = "Username required.")]
          public string Username { get; set; } = null!;

          [RegularExpression(@"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*()_=+';:.>|<,]).{8,32}$", ErrorMessage = "Password too weak.")]
          [Required(ErrorMessage = "Password required.")]
          public string Password { get; set; } = null!;

          [Required(ErrorMessage = "Email required.")]
          [DataType(DataType.EmailAddress)]
          [EmailAddress]
          public string Email { get; set; } = null!;
     }
}
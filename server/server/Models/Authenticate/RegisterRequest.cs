using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models.Authenticate
{
    public class RegisterRequest
    {
        [StringLength(32, MinimumLength = 6, ErrorMessage = "Username too short.")]
        [Required(ErrorMessage = "Username required.")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Email required.")]
        [EmailAddress(ErrorMessage = "Email incorrect.")]
        public string Email { get; set; } = null!;

        [RegularExpression(@"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[!-\/:-@[-\`]).{8,32}$", ErrorMessage = "Password too weak.")]
        [Required(ErrorMessage = "Password required.")]
        public string Password { get; set; } = null!;

        [NotMapped]
        [Required(ErrorMessage = "Password required.")]
        [Compare(nameof(Password), ErrorMessage = "Password doesn't match.")]
        public string RepeatPassword { get; set; } = null!;
        
        public int? FTP { get; set; }
        public int? HrRest { get; set; }
        public int? HrMax { get; set; }
    }
}

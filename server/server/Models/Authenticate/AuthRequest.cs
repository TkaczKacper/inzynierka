using System.ComponentModel.DataAnnotations;

namespace server.Models.Authenticate
{
    public class AuthRequest
    {
        [StringLength(32, MinimumLength = 6, ErrorMessage = "Username too short.")]
        [Required(ErrorMessage = "Username required.")]
        public string Username { get; set; } = null!;

        [RegularExpression(@"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[!-\/:-@[-\`]).{8,32}$", ErrorMessage = "Password too weak.")]
        [Required(ErrorMessage = "Password required.")]
        public string Password { get; set; } = null!;

    }
}

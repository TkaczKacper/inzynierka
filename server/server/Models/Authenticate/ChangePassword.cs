namespace server.Models.Authenticate;

public class ChangePassword
{
    public required string OldPassword { get; set; }
    public required string NewPassword { get; set; }
}
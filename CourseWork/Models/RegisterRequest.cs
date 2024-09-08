namespace CourseWork.Models;

public class RegisterRequest
{
    public string Email { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public string PasswordApprove { get; set; }
    public bool Remember { get; set; }
}
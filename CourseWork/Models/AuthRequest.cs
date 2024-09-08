namespace CourseWork.Models;

public class AuthRequest
{
    public string Login { get; set; }
    public string Password { get; set; }
    public bool Remember { get; set; }
    public string ReturnUrl { get; set; }
}
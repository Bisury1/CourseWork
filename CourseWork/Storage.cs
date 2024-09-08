using Microsoft.AspNetCore.Identity;

namespace CourseWork;

public static class Storage
{
    public static IdentityUser User = new()
    {
        Email = "fnebaev@mail.ru",
        UserName = "a.nebaev"
    };

    public static string UserPassword = "123Az_";

    public static string Email = "fnebaev@mail.ru";
}
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace CourseWork.Controllers;

public class BaseAppController : ControllerBase
{
    internal Guid UserId => !User.Identity.IsAuthenticated
        ? Guid.Empty
        : Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
}
using CourseWork.Domain.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CourseWork.Controllers;

/// <summary>
/// Только для админа
/// </summary>
/// <param name="userManager"></param>
[Authorize(Roles = nameof(UserRoles.Admin))]
public class AdminV1Controller(UserManager<IdentityUser> userManager) : ControllerBase
{
    /// <summary>
    /// Добавление пользователя в админы
    /// </summary>
    /// <param name="email">Почта пользователя</param>
    /// <returns></returns>
    [HttpPatch]
    [Route("/v1/admin/users/roles/edit")]
    public async Task<IActionResult> EditRoleToAdmin([FromForm] string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return BadRequest();
        
        var result = await userManager.AddToRoleAsync(user, nameof(UserRoles.Admin));
        return result.Succeeded
            ? Ok()
            : BadRequest();
    }

    /// <summary>
    /// Удаление пользователя
    /// </summary>
    /// <param name="email">Почта пользователя</param>
    /// <returns></returns>
    [HttpDelete]
    [Route("/v1/admin/users/delete")]
    public async Task<IActionResult> DeleteUser([FromForm] string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return BadRequest();
        
        var result = await userManager.DeleteAsync(user);
        return result.Succeeded
            ? Ok()
            : BadRequest();
    }
}
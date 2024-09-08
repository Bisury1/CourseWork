using CourseWork.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RegisterRequest = CourseWork.Models.RegisterRequest;

namespace CourseWork.Controllers;

public class UserV1Controller(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager) : ControllerBase
{
    /// <summary>
    /// Авторизация пользвоателей
    /// </summary>
    /// <param name="authRequest">Данные для авторизации</param>
    /// <returns></returns>
    [HttpPost]
    [Route("/v1/users/auth")]
    public async Task<IActionResult> Auth([FromBody] AuthRequest authRequest)
    {
        var result =
            await signInManager.PasswordSignInAsync(authRequest.Login, authRequest.Password, 
                authRequest.Remember,false);
        
        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(authRequest.ReturnUrl))
                return Redirect(authRequest.ReturnUrl);

            return Ok();
        }

        return BadRequest("Неправильный логин или пароль");
    }

    /// <summary>
    /// Регистрация пользователей
    /// </summary>
    /// <param name="registerRequest">Данные для регистрации</param>
    /// <returns></returns>
    [HttpPost]
    [Route("/v1/users/register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
    {
        if (registerRequest.Password != registerRequest.PasswordApprove)
            return BadRequest("Пароль отличается");

        var user = new IdentityUser { UserName = registerRequest.Login, Email = registerRequest.Email };
        var result = await userManager.CreateAsync(user, registerRequest.Password);

        if (result.Succeeded)
        {
            await signInManager.SignInAsync(user, registerRequest.Remember);
            return Ok();
        }

        return BadRequest(string.Join(" ", result.Errors.Select(e => e.Description)));
    }

    /// <summary>
    /// Выход из аккаунта
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    [Route("/v1/users/auth/logout")]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return Ok();
    }
}
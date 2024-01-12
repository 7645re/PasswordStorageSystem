using Domain.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTO.Request;
using WebAPI.Mappers;

namespace WebAPI.Controllers;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LogInAsync([FromBody] UserLogInRequest userLogInRequest)
    {
        var result = await _userService.GetUserTokenAsync(userLogInRequest.ToUserLogIn());
        return Ok(result.ToTokenInfoResponse());
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] UserRegisterRequest userRegisterRequest)
    {
        var result = await _userService.CreateUserAsync(userRegisterRequest.ToUserCreate());
        return Ok(result.ToTokenInfoResponse());
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetUserAsync()
    {
        var result = await _userService.GetUserAsync(User.Identity.Name);
        return Ok(result.ToUserResponse());
    }

    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> DeleteUserAsync()
    {
        await _userService.DeleteUserAsync(User.Identity.Name);
        return Ok();
    }

    [Authorize]
    [HttpPatch("password")]
    public async Task<IActionResult> ChangeUserPasswordAsync(
        [FromBody] UserChangePasswordRequest userChangePasswordRequest)
    {
        await _userService.ChangePasswordAsync(userChangePasswordRequest.ToUserChangePassword());
        return Ok();
    }
}
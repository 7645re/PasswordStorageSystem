using Domain.Services;
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

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _userService.GetAllUsersAsync();
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LogInAsync([FromBody] UserLogInRequest userLogInRequest)
    {
        var result = await _userService.GetUserTokenAsync(userLogInRequest.ToUserLogIn());
        if (!result.IsSuccess) return BadRequest(result);

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] UserRegisterRequest userRegisterRequest)
    {
        var result = await _userService.CreateUserAsync(userRegisterRequest.ToUserCreate());
        if (!result.IsSuccess) return BadRequest(result);

        return Ok(result);
    }

    [Authorize]
    [HttpGet("info")]
    public async Task<IActionResult> GetUserAsync()
    {
        var userLogin = User.Identity?.Name;
        if (userLogin == null) return BadRequest("Server error token doesnt have user login");

        var result = await _userService.GetUserAsync(userLogin);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("delete")]
    public async Task<IActionResult> DeleteUserAsync()
    {
        var userLogin = User.Identity?.Name;
        if (userLogin == null) return BadRequest("Server error token doesnt have user login");

        var result = await _userService.DeleteUserAsync(userLogin);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("password/change")]
    public async Task<IActionResult> ChangeUserPasswordAsync(
        [FromBody] UserChangePasswordRequest userChangePasswordRequest)
    {
        var userLogin = User.Identity?.Name;
        if (userLogin == null) return BadRequest("Server error token doesnt have user login");

        var result = await _userService.ChangePasswordAsync(userChangePasswordRequest.ToUserChangePassword());
        return Ok(result);
    }
}
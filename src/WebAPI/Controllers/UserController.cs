using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTO.Request;

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
    public async Task<IActionResult> LoginAsync([FromBody] UserSearchRequest userSearchRequest)
    {
        var result = await _userService.GetUserAsync(userSearchRequest.Login);
        if (!result.IsSuccess) return BadRequest(result);

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] UserCreateRequest userCreateRequest)
    {
        var result = await _userService.CreateUserAsync(userCreateRequest.Login, userCreateRequest.Password);
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
    public async Task<IActionResult> ChangeUserPasswordAsync([FromBody] UserUpdateRequest userUpdateRequest)
    {
        var userLogin = User.Identity?.Name;
        if (userLogin == null) return BadRequest("Server error token doesnt have user login");

        var result = await _userService.ChangePasswordAsync(userLogin, userUpdateRequest.NewPassword);
        return Ok(result);
    }
}
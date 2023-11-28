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
    public async Task<IActionResult> LoginAsync([FromBody] UserSearch userSearch)
    {
        var result = await _userService.GetUserByLoginAndPasswordAsync(userSearch.UserLogin, userSearch.Password);
        if (!result.IsSuccess) return BadRequest(result);

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] UserCreate userCreate)
    {
        var result = await _userService.CreateUserAsync(userCreate.UserLogin, userCreate.Password);
        if (!result.IsSuccess) return BadRequest(result);

        return Ok(result);
    }
    
    [Authorize]
    [HttpGet("{userLogin}")]
    public async Task<IActionResult> GetUserAsync(string userLogin)
    {
        var result = await _userService.GetUserAsync(userLogin);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("{userLogin}/delete")]
    public async Task<IActionResult> DeleteUserAsync(string userLogin)
    {
        var result = await _userService.DeleteUserAsync(userLogin);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("{userLogin}/password/change")]
    public async Task<IActionResult> ChangeUserPasswordAsync(string userLogin,
        [FromBody] UserPasswordUpdate userPasswordUpdate)
    {
        var result = await _userService.ChangePasswordAsync(userLogin, userPasswordUpdate.NewPassword);
        return Ok(result);
    }
}
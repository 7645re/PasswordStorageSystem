using Domain.DTO;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTO.Request;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("/register")]
    public async Task<IActionResult> RegisterAsync([FromBody] UserCreateRequest userCreateRequest)
    {
        var result =
            await _userService.CreateUserAsync(new UserToCreate(userCreateRequest.Login, userCreateRequest.Password));
        return Ok(result);
    }

    [HttpPost("/changePassword")]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] UserChangePasswordRequest userChangePasswordRequest)
    {
        var result = await _userService.ChangePasswordByLoginAsync(new UserToChangePassword(
            userChangePasswordRequest.Login,
            userChangePasswordRequest.NewPassword));
        return Ok(result);
    }
}
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTO.Response;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PasswordManagerController : ControllerBase
{
    private readonly IPasswordManagerService _passwordManagerService;
    
    public PasswordManagerController(IPasswordManagerService passwordManagerService)
    {
        _passwordManagerService = passwordManagerService;
    }

    [HttpGet("/passwords")]
    public async Task<IActionResult> GetPasswordsAsync([FromQuery] string login)
    {
        var result = await _passwordManagerService.GetAllPasswordsAsync(login);
        return Ok(result.Select(r => new ResourcePasswordResponse(r.ResourceName, r.ResourcePassword)));
    }
}
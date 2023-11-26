using System;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTO;
using WebAPI.DTO.Request;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CredentialsManagerController : ControllerBase
{
    private readonly ICredentialManagerService _credentialManagerService;

    public CredentialsManagerController(ICredentialManagerService credentialManagerService)
    {
        _credentialManagerService = credentialManagerService;
    }

    [HttpGet("/credentials")]
    public async Task<IActionResult> GetCredentialsAsync([FromQuery] string login)
    {
        var result = await _credentialManagerService.GetAllUserCredentialsAsync(login);
        return Ok(result);
    }
    
    [HttpGet("/credentials/all")]
    public async Task<IActionResult> GetCredentialsAsync()
    {
        var result = await _credentialManagerService.GetAll();
        return Ok(result);
    }
    
    [HttpGet("/credentials/filter")]
    public async Task<IActionResult> GetCredentialsByAtDateAsync([FromQuery] DateTimeOffset date)
    {
        var result = await _credentialManagerService.GetCredentialsByAtDate(date);
        return Ok(result);
    }

    [HttpPost("/credentials/create")]
    public async Task<IActionResult> CreateCredentialAsync(
        [FromBody] ResourceCredentialCreateRequest request)
    {
        var result = await _credentialManagerService.CreateUserCredentialAsync(
            new ResourceCredentialToCreate(request.Login,
                request.ResourceName, request.ResourceLogin,
                request.ResourcePassword));
        return Ok(result);
    }
    
    [HttpGet("/credentials/generate")]
    public async Task<IActionResult> GenerateCredentialAsync([FromQuery] string login)
    {
        var result = await _credentialManagerService.GenerateUserCredentialAsync(login);
        return Ok(result);
    }
    
    [HttpPost("/credentials/change")]
    public async Task<IActionResult> ChangeCredentialAsync([FromBody] ResourceCredentialChangeRequest request)
    {
        var result = await _credentialManagerService.ChangeUserCredentialAsync(new ResourceCredentialToChange(
            request.UserLogin,
            request.ResourceName,
            request.ResourceLogin,
            request.NewResourceLogin,
            request.NewResourcePassword));
        return Ok(result);
    }
}
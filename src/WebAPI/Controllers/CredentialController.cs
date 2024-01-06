namespace WebAPI.Controllers;

using Domain.Services.CredentialService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTO.Request;
using WebAPI.Mappers;

[ApiController]
[Authorize]
[Route("/credentials")]
public class CredentialController : ControllerBase
{
    private readonly ICredentialService _credentialService;

    public CredentialController(ICredentialService credentialService)
    {
        _credentialService = credentialService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUserCredentialsAsync(
        [FromQuery] int pageSize, int pageNumber)
    {
        var result = await _credentialService.GetCredentialsAsync(
            User.Identity?.Name, pageSize, pageNumber);
        return Ok(result);
    }

    [HttpDelete("all")]
    public async Task<IActionResult> DeleteAllUserCredentialsAsync()
    {
        await _credentialService.DeleteUserCredentialAsync(User.Identity?.Name);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteCredentialAsync(
        [FromBody] CredentialDeleteRequest credentialDeleteRequest)
    {
        await _credentialService.DeleteCredentialAsync(
            credentialDeleteRequest.ToCredentialDelete(User.Identity?.Name));
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> CreateCredentialAsync(
        [FromBody] CredentialCreateRequest credentialCreateRequest)
    {
        var result =
            await _credentialService.CreateCredentialAsync(
                credentialCreateRequest.ToCredentialCreate(User.Identity?.Name));
        return Ok(result);
    }
    
    [HttpPatch]
    public async Task<IActionResult> UpdateCredentialAsync(
        [FromBody] CredentialUpdateRequest credentialUpdateRequest)
    {
        var result =
            await _credentialService.ChangeCredentialPasswordAsync(
                credentialUpdateRequest.ToCredentialUpdate(User.Identity?.Name));
        return Ok(result);
    }
}
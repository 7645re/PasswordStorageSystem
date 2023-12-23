using Domain.Services.CredentialService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTO.Request;
using WebAPI.Mappers;

namespace WebAPI.Controllers;

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

    [HttpGet("count")]
    public async Task<IActionResult> GetCredentialsCountAsync()
    {
        var result = await _credentialService.GetCredentialsCountAsync(User.Identity.Name);
        return Ok(result);
    }

    [HttpGet("passwords-security-levels")]
    public async Task<IActionResult> GetPasswordsLevelsInfoAsync()
    {
        var result = await _credentialService.GetPasswordsLevelsInfoAsync(User.Identity.Name);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetCredentialsAsync()
    {
        var result = await _credentialService.GetCredentialsAsync(User.Identity.Name);
        return Ok(result);
    }

    [HttpGet("{id}/history")]
    public async Task<IActionResult> GetCredentialHistoryAsync(Guid id)
    {
        var result = await _credentialService.GetCredentialHistoryAsync(id);
        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteCredentialAsync([FromBody] CredentialDeleteRequest credentialDeleteRequest)
    {
        await _credentialService.DeleteCredentialAsync(
                credentialDeleteRequest.ToCredentialDelete(User.Identity.Name));
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> CreateCredentialAsync([FromBody] CredentialCreateRequest credentialCreateRequest)
    {
        var result =
            await _credentialService.CreateCredentialAsync(
                credentialCreateRequest.ToCredentialCreate(User.Identity.Name));
        return Ok(result);
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateCredentialAsync([FromQuery] int count)
    {
        var result = await _credentialService.GenerateCredentialsAsync(User.Identity.Name, count);
        return Ok(result);
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateCredentialAsync([FromBody] CredentialUpdateRequest credentialUpdateRequest)
    {
        var result =
            await _credentialService.UpdateCredentialAsync(
                credentialUpdateRequest.ToCredentialUpdate(User.Identity.Name));
        return Ok(result);
    }
}
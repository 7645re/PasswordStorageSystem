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
    [HttpGet("count")]
    public async Task<IActionResult> GetCredentialsCountAsync()
    {
        return Ok();
    }

    [HttpGet("passwords-security-levels")]
    public async Task<IActionResult> GetPasswordsLevelsInfoAsync()
    {
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetCredentialsAsync()
    {
        return Ok();
    }

    [HttpGet("{id}/history")]
    public async Task<IActionResult> GetCredentialHistoryAsync(Guid id)
    {
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteCredentialAsync([FromBody] CredentialDeleteRequest credentialDeleteRequest)
    {
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> CreateCredentialAsync([FromBody] CredentialCreateRequest credentialCreateRequest)
    {
        return Ok();
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateCredentialAsync([FromQuery] int count)
    {
        return Ok();
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateCredentialAsync([FromBody] CredentialUpdateRequest credentialUpdateRequest)
    {
        return Ok();
    }
}
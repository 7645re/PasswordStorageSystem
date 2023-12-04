using Domain.Services;
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
        // TODO: Unify part with get user login and return bad request
        var userLogin = User.Identity?.Name;
        if (userLogin == null) return BadRequest("Server error token doesnt have user login");
        var result = await _credentialService.GetCredentialsCountAsync(userLogin);
        return Ok(result);
    }
    
    [HttpGet("passwordsSecurityLevels")]
    public async Task<IActionResult> GetPasswordsLevelsInfoAsync()
    {
        var userLogin = User.Identity?.Name;
        if (userLogin == null) return BadRequest("Server error token doesnt have user login");
        var result = await _credentialService.GetPasswordsLevelsInfoAsync(userLogin);
        return Ok(result);
    }    
    
    [HttpGet]
    public async Task<IActionResult> GetCredentialsAsync()
    {
        var userLogin = User.Identity?.Name;
        if (userLogin == null) return BadRequest("Server error token doesnt have user login");
        var result = await _credentialService.GetCredentialsAsync(userLogin);
        return Ok(result);
    }

    [HttpPost("delete")]
    public async Task<IActionResult> DeleteCredentialAsync([FromBody] CredentialDeleteRequest credentialDeleteRequest)
    {
        var userLogin = User.Identity?.Name;
        if (userLogin == null) return BadRequest("Server error token doesnt have user login");
        var result = await _credentialService.DeleteCredentialAsync(credentialDeleteRequest.ToCredentialDelete(userLogin));
        return Ok(result);
    }

    [HttpGet("delete")]
    public async Task<IActionResult> DeleteCredentialByResourceAsync([FromQuery] string resourceName)
    {
        var userLogin = User.Identity?.Name;
        if (userLogin == null) return BadRequest("Server error token doesnt have user login");
        var result = await _credentialService.DeleteResourceCredentialsAsync(userLogin, resourceName);
        return Ok(result);
    }

    [HttpPost("delete/all")]
    public async Task<IActionResult> DeleteCredentialAsync()
    {
        var userLogin = User.Identity?.Name;
        if (userLogin == null) return BadRequest("Server error token doesnt have user login");
        var result = await _credentialService.DeleteAllCredentialsAsync(userLogin);
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateCredentialAsync([FromBody] CredentialCreateRequest credentialCreateRequest)
    {
        var userLogin = User.Identity?.Name;
        if (userLogin == null) return BadRequest("Server error token doesnt have user login");
        var result = await _credentialService.CreateCredentialAsync(credentialCreateRequest.ToCredentialCreate(userLogin));
        return Ok(result);
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateCredentialAsync([FromQuery] int count)
    {
        var userLogin = User.Identity?.Name;
        if (userLogin == null) return BadRequest("Server error token doesnt have user login");
        var result = await _credentialService.GenerateCredentialsAsync(userLogin, count);
        return Ok(result);
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateCredentialAsync([FromBody] CredentialUpdateRequest credentialUpdateRequest)
    {
        var userLogin = User.Identity?.Name;
        if (userLogin == null) return BadRequest("Server error token doesnt have user login");
        var result = await _credentialService.UpdateCredentialAsync(credentialUpdateRequest.ToCredentialUpdate(userLogin));
        return Ok(result);
    }
}
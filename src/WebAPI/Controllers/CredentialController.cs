using System.Threading.Tasks;
using Domain.DTO;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTO.Request;
using CredentialCreate = WebAPI.DTO.Request.CredentialCreate;

namespace WebAPI.Controllers;

[ApiController]
[Authorize]
[Route("user/{userLogin}/credentials")]
public class CredentialController : ControllerBase
{
    private readonly ICredentialService _credentialService;

    public CredentialController(ICredentialService credentialService)
    {
        _credentialService = credentialService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCredentialsAsync(string userLogin)
    {
        var result = await _credentialService.GetCredentialsAsync(userLogin);
        return Ok(result);
    }

    [HttpPost("delete")]
    public async Task<IActionResult> DeleteCredentialAsync(string userLogin,
        [FromBody] CredentialDelete credentialDelete)
    {
        var result = await _credentialService.DeleteCredentialAsync(userLogin, credentialDelete.ResourceName,
            credentialDelete.ResourceLogin);
        return Ok(result);
    }

    [HttpGet("delete")]
    public async Task<IActionResult> DeleteCredentialByResourceAsync(string userLogin, [FromQuery] string resourceName)
    {
        var result = await _credentialService.DeleteResourceCredentialsAsync(userLogin, resourceName);
        return Ok(result);
    }

    [HttpPost("delete/all")]
    public async Task<IActionResult> DeleteCredentialAsync(string userLogin)
    {
        var result = await _credentialService.DeleteAllCredentialsAsync(userLogin);
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> DeleteCredentialAsync(string userLogin,
        [FromBody] CredentialCreate credentialCreate)
    {
        var result = await _credentialService.CreateCredentialAsync(new Domain.DTO.CredentialCreate(
            userLogin,
            credentialCreate.ResourceName,
            credentialCreate.ResourceLogin,
            credentialCreate.ResourcePassword));
        return Ok(result);
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateCredentialAsync(string userLogin, [FromQuery] int count)
    {
        var result = await _credentialService.GenerateCredentialsAsync(userLogin, count);
        return Ok(result);
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateCredentialAsync(string userLogin,
        [FromBody] UpdateCredential updateCredential)
    {
        var result = await _credentialService.UpdateCredentialAsync(new CredentialUpdate(
            userLogin,
            updateCredential.ResourceName,
            updateCredential.ResourceLogin,
            updateCredential.NewResourcePassword));
        return Ok(result);
    }
}
using Domain.DTO.Credential;
using Domain.Enums;
using WebAPI.DTO.Request;

namespace WebAPI.Mappers;

public static class CredentialMapper
{
    public static CredentialCreate ToCredentialCreate(this CredentialCreateRequest createRequest, string userLogin)
    {
        return new CredentialCreate(
            userLogin,
            createRequest.ResourceName,
            createRequest.ResourceLogin,
            createRequest.ResourcePassword);
    }

    public static CredentialDelete ToCredentialDelete(this CredentialDeleteRequest credentialDeleteRequest,
        string userLogin)
    {
        return new CredentialDelete(
            userLogin,
            credentialDeleteRequest.ResourceName,
            credentialDeleteRequest.ResourceLogin,
            credentialDeleteRequest.PasswordSecurityLevel,
            credentialDeleteRequest.CreatedAt,
            credentialDeleteRequest.Id);
    }
}
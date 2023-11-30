using Domain.DTO;
using WebAPI.DTO.Request;

namespace WebAPI.Mappers;

public static class CredentialMapper
{
    public static CredentialDelete ToCredentialDelete(this CredentialDeleteRequest credentialDeleteRequest,
        string userLogin)
    {
        return new CredentialDelete(userLogin, credentialDeleteRequest.ResourceName,
            credentialDeleteRequest.ResourceLogin);
    }
    
    public static CredentialCreate ToCredentialCreate(this CredentialCreateRequest credentialCreateRequest,
        string userLogin)
    {
        return new CredentialCreate(userLogin,
            credentialCreateRequest.ResourceName,
            credentialCreateRequest.ResourceLogin,
            credentialCreateRequest.ResourcePassword);
    }
    
    public static CredentialUpdate ToCredentialUpdate(this CredentialUpdateRequest credentialUpdateRequest,
        string userLogin)
    {
        return new CredentialUpdate(userLogin,
            credentialUpdateRequest.ResourceName,
            credentialUpdateRequest.ResourceLogin,
            credentialUpdateRequest.NewResourcePassword);
    }
}
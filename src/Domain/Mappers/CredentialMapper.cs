using Domain.DTO.Credential;
using Domain.Enums;
using Domain.Models;

namespace Domain.Mappers;

public static class CredentialMapper
{
    public static CredentialByResourceEntity ToCredentialByResourceEntity(this CredentialEntity credentialEntity)
    {
        return new CredentialByResourceEntity
        {
            UserLogin = credentialEntity.UserLogin,
            ResourceName = credentialEntity.ResourceName,
            ResourceLogin = credentialEntity.ResourceLogin
        };
    }

    public static CredentialCountBySecurityLevelEntity ToCredentialCountBySecurityLevelEntity(
        this CredentialEntity credentialEntity)
    {
        return new CredentialCountBySecurityLevelEntity
        {
            UserLogin = credentialEntity.UserLogin,
            PasswordSecurityLevel = credentialEntity.PasswordSecurityLevel,
            Count = 0
        };
    }

    public static CredentialEntity ToCredentialEntity(this Credential credential)
    {
        return new CredentialEntity
        {
            UserLogin = credential.UserLogin,
            CreatedAt = credential.CreatedAt,
            Id = credential.Id,
            ResourceName = credential.ResourceName,
            ResourceLogin = credential.ResourceLogin,
            ResourcePassword = credential.ResourcePassword,
            PasswordSecurityLevel = credential.PasswordSecurityLevel,
            ChangedAt = credential.ChangedAt
        };
    }
}
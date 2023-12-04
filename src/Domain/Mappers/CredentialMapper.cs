using Domain.DTO;
using Domain.Enums;
using Domain.Models;

namespace Domain.Mappers;

public static class CredentialMapper
{
    public static Credential ToCredential(
        this CredentialEntity credentialEntity)
    {
        return new Credential
        {
            ResourceName = credentialEntity.ResourceName,
            ResourceLogin = credentialEntity.ResourceLogin,
            ResourcePassword = credentialEntity.ResourcePassword,
            CreateAt = credentialEntity.CreatedAt.ToLocalTime(),
            ChangeAt = credentialEntity.ChangeAt?.ToLocalTime(),
            History = Array.Empty<CredentialHistoryItem>(),
            PasswordSecurityLevel = credentialEntity.PasswordSecurityLevel
        };
    }

    public static CredentialEntity ToCredentialEntity(
        this CredentialUpdate credentialUpdate,
        PasswordSecurityLevel passwordSecurityLevel,
        DateTimeOffset createAt,
        DateTimeOffset changeAt)
    {
        return new CredentialEntity
        {
            UserLogin = credentialUpdate.UserLogin,
            ResourceName = credentialUpdate.ResourceName,
            ResourceLogin = credentialUpdate.ResourceLogin,
            ResourcePassword = credentialUpdate.NewResourcePassword,
            PasswordSecurityLevel = passwordSecurityLevel,
            CreatedAt = createAt,
            ChangeAt = changeAt
        };
    }

    public static CredentialEntity ToCredentialEntity(
        this CredentialCreate credentialCreate,
        PasswordSecurityLevel passwordSecurityLevel,
        DateTimeOffset createAt)
    {
        return new CredentialEntity
        {
            UserLogin = credentialCreate.UserLogin,
            ResourceName = credentialCreate.ResourceName,
            ResourceLogin = credentialCreate.ResourceLogin,
            ResourcePassword = credentialCreate.ResourcePassword,
            PasswordSecurityLevel = passwordSecurityLevel,
            CreatedAt = createAt,
            ChangeAt = null
        };
    }

    public static CredentialHistoryItem ToCredentialHistoryItem(
        this CredentialHistoryItemEntity credentialHistoryItemEntity)
    {
        return new CredentialHistoryItem(
            credentialHistoryItemEntity.ResourceName,
            credentialHistoryItemEntity.ResourceLogin,
            credentialHistoryItemEntity.ResourcePassword,
            credentialHistoryItemEntity.ChangeAt.ToLocalTime()
        );
    }

    public static CredentialHistoryItemEntity ToCredentialHistoryItemEntity(
        this CredentialEntity credentialEntity)
    {
        return new CredentialHistoryItemEntity
        {
            UserLogin = credentialEntity.UserLogin,
            ResourceName = credentialEntity.ResourceName,
            ResourceLogin = credentialEntity.ResourceLogin,
            ResourcePassword = credentialEntity.ResourcePassword,
            ChangeAt = DateTimeOffset.UtcNow
        };
    }
}
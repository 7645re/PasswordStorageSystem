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
            CreatedAt = credentialEntity.CreatedAt.ToLocalTime(),
            ChangedAt = credentialEntity.ChangedAt?.ToLocalTime(),
            PasswordSecurityLevel = credentialEntity.PasswordSecurityLevel,
            Id = credentialEntity.Id
        };
    }

    public static CredentialEntity ToCredentialEntity(
        this CredentialUpdate credentialUpdate,
        PasswordSecurityLevel passwordSecurityLevel,
        DateTimeOffset createdAt,
        DateTimeOffset changedAt)
    {
        return new CredentialEntity
        {
            UserLogin = credentialUpdate.UserLogin,
            ResourceName = credentialUpdate.ResourceName,
            ResourceLogin = credentialUpdate.ResourceLogin,
            ResourcePassword = credentialUpdate.NewResourcePassword,
            PasswordSecurityLevel = passwordSecurityLevel,
            CreatedAt = createdAt,
            ChangedAt = changedAt
        };
    }

    public static CredentialEntity ToCredentialEntity(
        this CredentialCreate credentialCreate,
        PasswordSecurityLevel passwordSecurityLevel,
        DateTimeOffset createdAt,
        Guid id)
    {
        return new CredentialEntity
        {
            UserLogin = credentialCreate.UserLogin,
            ResourceName = credentialCreate.ResourceName,
            ResourceLogin = credentialCreate.ResourceLogin,
            ResourcePassword = credentialCreate.ResourcePassword,
            PasswordSecurityLevel = passwordSecurityLevel,
            CreatedAt = createdAt,
            Id = id,
            ChangedAt = null
        };
    }

    public static CredentialHistoryItem ToCredentialHistoryItem(
        this CredentialHistoryItemEntity credentialHistoryItemEntity)
    {
        return new CredentialHistoryItem(
            credentialHistoryItemEntity.CredentialId,
            credentialHistoryItemEntity.ResourcePassword,
            credentialHistoryItemEntity.ChangedAt.ToLocalTime()
        );
    }

    public static CredentialHistoryItemEntity ToCredentialHistoryItemEntity(
        this CredentialEntity credentialEntity)
    {
        return new CredentialHistoryItemEntity
        {
            CredentialId = credentialEntity.Id,
            ResourcePassword = credentialEntity.ResourcePassword,
            ChangedAt = DateTimeOffset.UtcNow
        };
    }

    public static CredentialUpdated ToCredentialUpdated(this CredentialEntity credentialEntity, DateTimeOffset changedAt)
    {
        return new CredentialUpdated(credentialEntity.ResourcePassword, changedAt.ToLocalTime(), credentialEntity.PasswordSecurityLevel);
    }
}
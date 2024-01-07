using Domain.DTO.Credential;
using Domain.Enums;
using Domain.Models;

namespace Domain.Mappers;

public static class CredentialMapper
{
    public static CredentialByResourceEntity ToCredentialByResourceEntity(
        this CredentialEntity credentialEntity)
    {
        return new CredentialByResourceEntity
        {
            UserLogin = credentialEntity.UserLogin,
            ResourceName = credentialEntity.ResourceName,
            ResourceLogin = credentialEntity.ResourceLogin
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
            PasswordSecurityLevel = (int) credential.PasswordSecurityLevel,
            ChangedAt = credential.ChangedAt
        };
    }

    public static Credential ToCredential(this CredentialEntity credentialEntity)
    {
        return new Credential
        {
            UserLogin = credentialEntity.UserLogin,
            ResourceName = credentialEntity.ResourceName,
            ResourceLogin = credentialEntity.ResourceLogin,
            ResourcePassword = credentialEntity.ResourcePassword,
            CreatedAt = credentialEntity.CreatedAt,
            ChangedAt = credentialEntity.ChangedAt,
            PasswordSecurityLevel = (PasswordSecurityLevel) credentialEntity.PasswordSecurityLevel,
            Id = credentialEntity.Id
        };
    }

    public static CredentialUpdated ToCredentialUpdated(this CredentialEntity credentialEntity)
    {
        if (credentialEntity.ChangedAt is null)
            throw new InvalidOperationException("After updating the credential, the " +
                                                "time of the change was not specified");
        return new CredentialUpdated(
            credentialEntity.ResourcePassword,
            (DateTimeOffset) credentialEntity.ChangedAt,
            (PasswordSecurityLevel) credentialEntity.PasswordSecurityLevel);
    }

    public static CredentialEntity ToCredentialEntity(this CredentialDelete credentialDelete)
    {
        return new CredentialEntity
        {
            UserLogin = credentialDelete.UserLogin,
            ResourceName = credentialDelete.ResourceName,
            ResourceLogin = credentialDelete.ResourceLogin,
            CreatedAt = credentialDelete.CreatedAt,
            PasswordSecurityLevel = (int) credentialDelete.PasswordSecurityLevel,
            Id = credentialDelete.Id
        };
    }

    public static CredentialEntity ToCredentialEntity(
        this CredentialUpdate credentialUpdate,
        DateTimeOffset changedAt,
        PasswordSecurityLevel passwordSecurityLevel)
    {
        return new CredentialEntity
        {
            UserLogin = credentialUpdate.UserLogin,
            CreatedAt = credentialUpdate.CreatedAt,
            ResourcePassword = credentialUpdate.NewPassword,
            ChangedAt = changedAt,
            PasswordSecurityLevel = (int) passwordSecurityLevel,
            Id = credentialUpdate.Id
        };
    }

    public static CredentialHistoryItemEntity ToCredentialHistoryItem(
        this CredentialEntity credentialEntity)
    {
        if (credentialEntity.ChangedAt is null)
            throw new InvalidOperationException("After updating the credential, the " +
                                                "time of the change was not specified");

        return new CredentialHistoryItemEntity
        {
            CredentialId = credentialEntity.Id,
            ChangedAt = (DateTimeOffset) credentialEntity.ChangedAt,
            ResourcePassword = credentialEntity.ResourcePassword
        };
    }
}
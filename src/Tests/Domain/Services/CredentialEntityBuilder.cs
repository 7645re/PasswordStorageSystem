using Domain.Models;

namespace Tests.Domain.Services;

public class CredentialEntityBuilder
{
    private readonly CredentialEntity _entity = new();
    
    public CredentialEntityBuilder WithUserLogin(string userLogin)
    {
        _entity.UserLogin = userLogin;
        return this;
    }
    
    public CredentialEntityBuilder WithCreatedAt(DateTimeOffset createdAt)
    {
        _entity.CreatedAt = createdAt;
        return this;
    }
    
    public CredentialEntityBuilder WithId(Guid id)
    {
        _entity.Id = id;
        return this;
    }
    
    public CredentialEntityBuilder WithResourceName(string resourceName)
    {
        _entity.ResourceName = resourceName;
        return this;
    }
    
    public CredentialEntityBuilder WithResourceLogin(string resourceLogin)
    {
        _entity.ResourceLogin = resourceLogin;
        return this;
    }
    
    public CredentialEntityBuilder WithResourcePassword(string resourcePassword)
    {
        _entity.ResourcePassword = resourcePassword;
        return this;
    }
    
    public CredentialEntityBuilder WithPasswordSecurityLevel(int passwordSecurityLevel)
    {
        _entity.PasswordSecurityLevel = passwordSecurityLevel;
        return this;
    }
    
    public CredentialEntityBuilder WithChangedAt(DateTimeOffset changeAt)
    {
        _entity.ChangedAt = changeAt;
        return this;
    }

    public CredentialEntity Build()
    {
        return _entity;
    }
}
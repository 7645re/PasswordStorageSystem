using Cassandra.Data.Linq;
using Domain.Enums;
using Domain.Models;

namespace Domain.Repositories;

public interface ICredentialBySecurityLevelRepository
{
    CqlCommand CreateCredentialBySecurityLevelQuery(CredentialEntity credentialEntity);

    CqlCommand DeleteCredentialBySecurityLevelQuery(CredentialEntity credentialEntity);

    Task<long> GetCountOfUserPasswordWithSecurityLevelAsync(string userLogin,
        int passwordSecurityLevel);
}
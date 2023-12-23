using Cassandra.Data.Linq;
using Domain.Models;

namespace Domain.Repositories.CredentialBySecurityLevelRepository;

public interface ICredentialBySecurityLevelRepository
{
    CqlCommand CreateCredentialBySecurityLevelQuery(CredentialEntity credentialEntity);

    CqlCommand DeleteCredentialBySecurityLevelQuery(CredentialEntity credentialEntity);

    Task<long> GetCountOfUserPasswordWithSecurityLevelAsync(string userLogin,
        int passwordSecurityLevel);
}
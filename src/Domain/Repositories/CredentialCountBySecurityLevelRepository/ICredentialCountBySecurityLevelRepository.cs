using Cassandra.Data.Linq;
using Domain.Models;

namespace Domain.Repositories.CredentialCountBySecurityLevelRepository;

public interface ICredentialCountBySecurityLevelRepository
{
    Task CreateCountersForEachSecurityLevelAsync(string userLogin);
    Task ResetAllUserSecurityLevelCounterAsync(string userLogin);
    Task IncrementCredentialCountBySecurityLevelAsync(CredentialEntity credentialEntity);
    CqlUpdate IncrementCredentialCountBySecurityLevelQuery(CredentialEntity credentialEntity);
    Task DecrementCredentialCountBySecurityLevelAsync(CredentialEntity credentialEntity);
    CqlUpdate DecrementCredentialCountBySecurityLevelQuery(CredentialEntity credentialEntity);
}
using Cassandra.Data.Linq;
using Domain.Models;

namespace Domain.Repositories.CredentialCountBySecurityLevelRepository;

public interface ICredentialCountBySecurityLevelRepository
{
    CqlCommand CreateCredentialCountBySecurityLevelQuery(
        CredentialCountBySecurityLevelEntity credentialCountBySecurityLevelEntity);

    CqlCommand DeleteCredentialCountBySecurityLevelQuery(CredentialEntity credentialEntity);

    CqlCommand IncrementCredentialCountBySecurityLevelQuery(CredentialEntity credentialEntity);

    CqlCommand DecrementCredentialCountBySecurityLevelQuery(CredentialEntity credentialEntity);
}
using Cassandra.Data.Linq;
using Domain.Models;

namespace Domain.Repositories.CredentialByPasswordRepository;

public interface ICredentialByPasswordRepository
{
    public CqlCommand CreateCredentialByPasswordQuery(CredentialEntity credentialEntity);

    public CqlCommand DeleteCredentialByPasswordQuery(CredentialEntity credentialEntity);

    public Task<IEnumerable<CredentialByPasswordEntity>> GetCredentialsByPasswordAsync(string resourcePassword);
}
using Cassandra.Data.Linq;
using Domain.Mappers;
using Domain.Models;
using Domain.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Domain.Repositories.CredentialByPasswordRepository;

public class CredentialByPasswordRepository : CassandraRepositoryBase<CredentialByPasswordEntity>,
    ICredentialByPasswordRepository
{
    public CredentialByPasswordRepository(IOptions<CassandraOptions> cassandraOptions,
        ILogger<CassandraRepositoryBase<CredentialByPasswordEntity>> logger) : base(cassandraOptions, logger)
    {
    }

    public CqlCommand CreateCredentialByPasswordQuery(CredentialEntity credentialEntity)
    {
        return AddQuery(credentialEntity.ToCredentialByPasswordEntity());
    }

    public CqlCommand DeleteCredentialByPasswordQuery(CredentialEntity credentialEntity)
    {
        return Table.Where(r =>
                r.ResourcePassword == credentialEntity.ResourcePassword
                && r.UserLogin == credentialEntity.UserLogin
                && r.ResourceName == credentialEntity.ResourceName
                && r.ResourceLogin == credentialEntity.ResourceLogin)
            .Delete();
    }

    public async Task<IEnumerable<CredentialByPasswordEntity>> GetCredentialsByPasswordAsync(string resourcePassword)
    {
        return await ExecuteQueryAsync(Table.Where(r =>
            r.ResourcePassword == resourcePassword));
    }
}
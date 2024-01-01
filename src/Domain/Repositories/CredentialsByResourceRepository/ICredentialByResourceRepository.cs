using Cassandra.Data.Linq;
using Domain.Models;

namespace Domain.Repositories.CredentialsByResourceRepository;

public interface ICredentialByResourceRepository
{
    CqlCommand CreateCredentialByResourceQuery(CredentialByResourceEntity credentialByResourceEntity);
    CqlCommand DeleteCredentialByResourceQuery(CredentialEntity credentialEntity);
    CqlCommand DeleteCredentialsByResourceQuery(string userLogin);
    Task<CredentialByResourceEntity?> TryGetCredentialByResourceAsync(
        CredentialByResourceEntity credentialByResourceEntity);
    Task<CredentialByResourceEntity> GetCredentialByResourceAsync(
        CredentialByResourceEntity credentialByResourceEntity);
}
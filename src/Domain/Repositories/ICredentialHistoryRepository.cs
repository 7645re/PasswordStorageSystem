using Cassandra.Data.Linq;
using Domain.Models;

namespace Domain.Repositories;

public interface ICredentialHistoryRepository
{
    Task CreateHistoryItemAsync(CredentialEntity credentialEntity);

    Task DeleteHistoryByCredentialIdAsync(Guid credentialId);

    CqlCommand DeleteHistoryByCredentialIdQuery(Guid credentialId);

    Task<IEnumerable<CredentialHistoryItemEntity>> GetHistoryByCredentialIdAsync(Guid credentialId);

    CqlCommand CreateHistoryItemQuery(CredentialEntity credentialEntity);
}
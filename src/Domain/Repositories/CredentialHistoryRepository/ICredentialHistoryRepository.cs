using Cassandra.Data.Linq;
using Domain.Models;

namespace Domain.Repositories.CredentialHistoryRepository;

public interface ICredentialHistoryRepository
{
    CqlCommand CreateCredentialHistoryItemQuery(CredentialEntity credentialEntity);
    CqlCommand DeleteCredentialHistoriesItemsQuery(Guid id);
    CqlCommand DeleteCredentialsHistoriesItemsQuery(IEnumerable<Guid> ids);
}
using Domain.Models;

namespace Domain.Repositories;

public interface ICredentialHistoryRepository
{
    Task CreateHistoryItemAsync(CredentialEntity credentialEntity);

    Task DeleteHistoryItemAsync(string userLogin, string resourceName, string resourceLogin,
        string resourcePassword);

    Task DeleteAllHistoryItemsByCredentialAsync(string userLogin, string resourceName,
        string resourceLogin);

    Task DeleteAllUserHistoryItemsAsync(string userLogin);

    Task DeleteAllUserHistoryItemsByResourceAsync(string userLogin, string resourceName);

    Task<IEnumerable<CredentialHistoryItemEntity>> GetHistoryItemByCredentialAsync(CredentialEntity credentialEntity);

    Task<IEnumerable<CredentialHistoryItemEntity>> GetHistoryItemByUserAsync(string userLogin);
}
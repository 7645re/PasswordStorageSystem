namespace Domain.Repositories.CredentialRepository;

public class PagingStatesByPageSize
{
    public int LastPageNumber { get; private set; } = 1;

    public byte[]? LastPagingState { get; private set; }

    private Dictionary<int, byte[]?> PagingStatesByPageNumber { get; } = new()
    {
        { 1, null }
    };

    public void AddLastPagingState(int pageNumber, byte[] pagingState)
    {
        if (LastPageNumber >= pageNumber)
            throw new ArgumentException("Paging state with such a number already set");

        PagingStatesByPageNumber.Add(pageNumber, pagingState);
        LastPageNumber = pageNumber;
        LastPagingState = pagingState;
    }

    public byte[]? GetPagingState(int pageNumber)
    {
        if (!PagingStatesByPageNumber.TryGetValue(pageNumber, out var pagingState))
            throw new ArgumentException("Paging state with such a number doesnt exist");
        return pagingState;
    }
}

public class CredentialPagingStateManager
{
    private readonly IDictionary<string, IDictionary<int, PagingStatesByPageSize>>
        _userCredentialPagingStates = new Dictionary<string, IDictionary<int, PagingStatesByPageSize>>();

    public PagingStatesByPageSize GetPagingStatesByPageSize(string login, int pageSize)
    {
        _userCredentialPagingStates.TryGetValue(login, out var credentialPagingStatesByLogin);
        if (credentialPagingStatesByLogin is null)
        {
            var pagingStatesByPageSize = new PagingStatesByPageSize();
            _userCredentialPagingStates.Add(login, new Dictionary<int, PagingStatesByPageSize>
            {
                {
                    pageSize, pagingStatesByPageSize
                }
            });
            return pagingStatesByPageSize;
        }

        credentialPagingStatesByLogin.TryGetValue(pageSize, out var credentialPagingStatesByPageSize);
        if (credentialPagingStatesByPageSize is null)
        {
            var pagingStatesByPageSize = new PagingStatesByPageSize();
            credentialPagingStatesByLogin.Add(pageSize, pagingStatesByPageSize);
            return pagingStatesByPageSize;
        }

        return credentialPagingStatesByPageSize;
    }

    public void Clear(string login)
    {
        _userCredentialPagingStates.Remove(login);
    }
}
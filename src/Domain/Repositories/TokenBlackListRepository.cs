using System.Data;
using Cassandra.Data.Linq;
using Domain.Models;
using Domain.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Domain.Repositories;

public class TokenBlackListRepository : CassandraRepositoryBase<TokenBlackListEntity>, ITokenBlackListRepository
{
    public TokenBlackListRepository(IOptions<CassandraOptions> cassandraOptions,
        ILogger<CassandraRepositoryBase<TokenBlackListEntity>> logger) : base(cassandraOptions, logger)
    {
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        var result = (await ExecuteQueryAsync(Table.Where(r => r.Token == token))).ToArray();
        if (result.Length == 1) return false;
        if (!result.Any()) return true;
        throw new DataException("Scheme exception");
    }
}
using System.Linq.Expressions;
using Cassandra;
using Cassandra.Data.Linq;
using Microsoft.Extensions.Options;

namespace Domain.Repositories;

public abstract class CassandraRepositoryBase<T> where T : class
{
    private readonly Table<T> _table;

    protected CassandraRepositoryBase(IOptions<ICassandraOptions> cassandraOptions)
    {
        var options = cassandraOptions.Value;
        var session = Cluster
            .Builder()
            .WithCredentials(options.UserName, options.Password)
            .WithDefaultKeyspace(options.KeySpace)
            .WithPort(options.Port)
            .AddContactPoint(options.Address)
            .Build()
            .Connect();

        _table = new Table<T>(session);
    }

    protected virtual async Task<T[]> GetByFilterAsync(Expression<Func<T, bool>> filter)
    {
        var result = await _table.Where(filter).ExecuteAsync();
        return result.ToArray();
    }

    public virtual async Task AddAsync(T entity)
    {
        await _table.Insert(entity).ExecuteAsync();
    }
}
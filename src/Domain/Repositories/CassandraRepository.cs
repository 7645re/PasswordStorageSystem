using System.Linq.Expressions;
using Cassandra;
using Cassandra.Data.Linq;
using Domain.Factories;
using Microsoft.Extensions.Logging;

namespace Domain.Repositories;

public abstract class CassandraRepositoryBase<T> where T : class
{
    protected Table<T> Table { get; set; }

    private readonly ILogger _logger;

    protected CassandraRepositoryBase(ICassandraSessionFactory sessionFactory,
        ILogger<CassandraRepositoryBase<T>> logger)
    {
        _logger = logger;
        var session = sessionFactory.GetSession();
        Table = new Table<T>(session);
        Table.CreateIfNotExists();
    }
    
    // constructor so that you can replace the table for tests
    protected CassandraRepositoryBase(Table<T> table, ILogger<CassandraRepositoryBase<T>> logger)
    {
        _logger = logger;
        Table = table;
    }
    
    private void LogQueryTrace(QueryTrace? queryTrace)
    {
        if (queryTrace == null) return;
        queryTrace.Parameters.TryGetValue("query", out var query);
        var message = string.Join(Environment.NewLine,
            queryTrace.Events.Select(e => e.Description));
        _logger.LogInformation(query + Environment.NewLine + message);
    }

    protected async Task<IEnumerable<T>> ExecuteQueryAsync(CqlQueryBase<T> query)
    {
        query.EnableTracing();
        var result = await query.ExecuteAsync();
        LogQueryTrace(query.QueryTrace);
        return result;
    }

    protected async Task<IEnumerable<TQuery>> ExecuteQueryAsync<TQuery>(CqlQueryBase<TQuery> query)
    {
        query.EnableTracing();
        var result = await query.ExecuteAsync();
        LogQueryTrace(query.QueryTrace);
        return result;
    }

    protected async Task<TQuery> ExecuteScalarQueryAsync<TQuery>(CqlScalar<TQuery> query)
    {
        query.EnableTracing();
        var result = await query.ExecuteAsync();
        LogQueryTrace(query.QueryTrace);
        return result;
    }

    protected async Task ExecuteQueryAsync(CqlDelete query)
    {
        query.EnableTracing();
        await query.ExecuteAsync();
        LogQueryTrace(query.QueryTrace);
    }

    protected async Task ExecuteAsBatchAsync(IReadOnlyCollection<CqlCommand> commands)
    {
        var batch = Table
            .GetSession()
            .CreateBatch()
            .Append(commands);
        await batch.ExecuteAsync();

        _logger.LogInformation(batch.ToString());
    }

    protected async Task AddAsync(T entity)
    {
        var query = Table.Insert(entity);
        query.EnableTracing();
        await query.ExecuteAsync();
        LogQueryTrace(query.QueryTrace);
    }

    protected CqlCommand AddQuery(T entity)
    {
        var query = Table.Insert(entity);
        query.EnableTracing();
        return query;
    }

    protected CqlCommand DeleteQuery(Expression<Func<T, bool>> predicate)
    {
        var query = Table
            .Where(predicate)
            .Delete();
        query.EnableTracing();
        return query;
    }

    protected async Task UpdateAsync(CqlUpdate query)
    {
        query.EnableTracing();
        await query.ExecuteAsync();
        LogQueryTrace(query.QueryTrace);
    }
}
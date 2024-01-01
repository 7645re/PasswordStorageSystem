using System.Linq.Expressions;
using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;
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

    protected CassandraRepositoryBase(Table<T> table, ILogger<CassandraRepositoryBase<T>> logger)
    {
        _logger = logger;
        Table = table;
    }

    private void LogQueryTrace(QueryTrace? queryTrace)
    {
        if (queryTrace == null) return;
        queryTrace.Parameters.TryGetValue("query", out var query);
        // TODO: stringBuilder
        var message = string.Join(Environment.NewLine,
            queryTrace.Events.Select(e => e.Description));
        _logger.LogInformation(query + Environment.NewLine + message);
    }

    protected async Task<IEnumerable<T>> ExecuteQueryAsync(CqlQuery<T> query)
    {
        query.EnableTracing();
        var result = await query.ExecuteAsync();
        LogQueryTrace(query.QueryTrace);
        return result;
    }

    protected async Task ExecuteQueryAsync(CqlUpdate query)
    {
        query.EnableTracing();
        await query.ExecuteAsync();
        LogQueryTrace(query.QueryTrace);
    }

    protected async Task ExecuteQueryAsync(CqlDelete query)
    {
        query.EnableTracing();
        await query.ExecuteAsync();
        LogQueryTrace(query.QueryTrace);
    }
    
    protected async Task ExecuteQueryAsync(CqlInsert<T> query)
    {
        query.EnableTracing();
        await query.ExecuteAsync();
        LogQueryTrace(query.QueryTrace);
    }
    
    protected async Task<long> ExecuteQueryAsync(CqlScalar<long> query)
    {
        query.EnableTracing();
        var result = await query.ExecuteAsync();
        LogQueryTrace(query.QueryTrace);
        return result;
    }

    protected async Task<IPage<T>> ExecuteQueryPagedAsync(CqlQuery<T> query)
    {
        query.EnableTracing();
        var result = await query.ExecutePagedAsync();
        LogQueryTrace(query.QueryTrace);
        return result;
    }

    protected async Task ExecuteAsBatchAsync(IEnumerable<CqlCommand> commands)
    {
        var batch = Table
            .GetSession()
            .CreateBatch()
            .Append(commands);
        await batch.ExecuteAsync();

        _logger.LogInformation(batch.ToString());
    }
}
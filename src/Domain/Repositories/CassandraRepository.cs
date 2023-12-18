using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;
using Cassandra.Mapping.TypeConversion;
using Domain.Enums;
using Domain.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Domain.Repositories;

public abstract class CassandraRepositoryBase<T> where T : class
{
    protected readonly Table<T> Table;

    private readonly ILogger _logger;

    protected CassandraRepositoryBase(IOptions<CassandraOptions> cassandraOptions,
        ILogger<CassandraRepositoryBase<T>> logger)
    {
        _logger = logger;
        var options = cassandraOptions.Value;
        var session = Cluster
            .Builder()
            .WithCredentials(options.UserName, options.Password)
            .WithPort(options.Port)
            .AddContactPoint(options.Address)
            .Build()
            .Connect();
        session.CreateKeyspaceIfNotExists(options.KeySpace);
        session.ChangeKeyspace(options.KeySpace);
        
        Table = new Table<T>(session);
        Table.CreateIfNotExists();
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
        // The library does not allow you to get a query trace from a executed batch
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
    
    protected async Task UpdateAsync(CqlUpdate query)
    {
        query.EnableTracing();
        await query.ExecuteAsync();
        LogQueryTrace(query.QueryTrace);
    }
}
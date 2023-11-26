using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Data.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Domain.Repositories;

public abstract class CassandraRepositoryBase<T> where T : class
{
    private readonly Table<T> _table;

    private readonly ILogger _logger;

    protected CassandraRepositoryBase(IOptions<ICassandraOptions> cassandraOptions, ILogger<CassandraRepositoryBase<T>> logger)
    {
        _logger = logger;
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
        _table.EnableTracing();
    }

    private void LogTrace(QueryTrace? queryTrace)
    {
        if (queryTrace != null)
        {
            queryTrace.Parameters.TryGetValue("query", out var query);
            var message = string.Join("\n",
                queryTrace.Events.Select(e => e.Description));
            _logger.LogInformation(query + "\n" + message);
        }
    }

    private void LogQuery(CqlQuery<T> query)
    {
        LogTrace(query.QueryTrace);
    }
    
    private void LogCommand(CqlCommand query)
    {
        LogTrace(query.QueryTrace);
    }

    protected async Task<T[]> GetByFilterAsync(Expression<Func<T, bool>> filter)
    {
        var query = _table.Where(filter);
        query.EnableTracing();
        var result = await query.ExecuteAsync();
        LogQuery(query);
        return result.ToArray();
    }

    public async Task<T[]> GetAllAsync()
    {
        var result = await _table.ExecuteAsync();
        return result.ToArray();
    }

    protected async Task AddAsync(T entity)
    {
        var query = _table.Insert(entity);
        query.EnableTracing();
        await query.ExecuteAsync();
        LogCommand(query);
    }
}
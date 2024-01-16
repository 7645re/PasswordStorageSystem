using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;
using Domain.Factories;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Domain.Repositories;

public abstract class CassandraRepositoryBase<T> where T : class
{
    protected Table<T> Table { get; }

    private readonly ILogger _logger;

    private readonly RetryPolicy _retryPolicy;

    protected CassandraRepositoryBase(
        ICassandraSessionFactory sessionFactory,
        ILogger<CassandraRepositoryBase<T>> logger)
    {
        _logger = logger;
        var session = sessionFactory.GetSession();
        Table = new Table<T>(session);

        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                2,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt) * 5),
                (exception, timespan, context) =>
                {
                    _logger.LogInformation($"{timespan}, {context.Count}, {exception}");
                });
        Table.CreateIfNotExists();
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
        return await _retryPolicy.ExecuteAsync(
            async () =>
            {
                query.EnableTracing();
                var result = await query.ExecuteAsync();
                LogQueryTrace(query.QueryTrace);
                return result;
            }
        );
    }

    protected async Task<IEnumerable<T>> ExecuteQueryAsync<T>(CqlQuery<T> query)
    {
        return await _retryPolicy.ExecuteAsync(
            async () =>
            {
                query.EnableTracing();
                var result = await query.ExecuteAsync();
                LogQueryTrace(query.QueryTrace);
                return result;
            }
        );
    }

    protected async Task ExecuteQueryAsync(CqlUpdate query)
    {
        await _retryPolicy.ExecuteAsync(
            async () =>
            {
                query.EnableTracing();
                await query.ExecuteAsync();
                LogQueryTrace(query.QueryTrace);
            }
        );
    }

    protected async Task ExecuteQueryAsync(CqlDelete query)
    {
        await _retryPolicy.ExecuteAsync(
            async () =>
            {
                query.EnableTracing();
                await query.ExecuteAsync();
                LogQueryTrace(query.QueryTrace);
            }
        );
    }

    protected async Task ExecuteQueryAsync(CqlInsert<T> query)
    {
        await _retryPolicy.ExecuteAsync(
            async () =>
            {
                query.EnableTracing();
                await query.ExecuteAsync();
                LogQueryTrace(query.QueryTrace);
            }
        );
    }

    protected async Task<long> ExecuteQueryAsync(CqlScalar<long> query)
    {
        return await _retryPolicy.ExecuteAsync(
            async () =>
            {
                query.EnableTracing();
                var result = await query.ExecuteAsync();
                LogQueryTrace(query.QueryTrace);
                return result;
            }
        );
    }

    protected async Task<IPage<T>> ExecuteQueryPagedAsync(CqlQuery<T> query)
    {
        return await _retryPolicy.ExecuteAsync(
            async () =>
            {
                query.EnableTracing();
                var result = await query.ExecutePagedAsync();
                LogQueryTrace(query.QueryTrace);
                return result;
            }
        );
    }

    protected async Task ExecuteAsBatchAsync(IEnumerable<CqlCommand> commands)
    {
        await _retryPolicy.ExecuteAsync(
            async () =>
            {
                var batch = Table
                    .GetSession()
                    .CreateBatch()
                    .Append(commands);
                await batch.ExecuteAsync();

                _logger.LogInformation(batch.ToString());
            });
    }
}
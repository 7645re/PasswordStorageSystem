using Cassandra;
using Domain.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;

namespace Domain.Factories;

public sealed class CassandraSessionFactory : ICassandraSessionFactory
{
    private Lazy<ISession> _session;
    private readonly ILogger<CassandraSessionFactory> _logger;
    

    public CassandraSessionFactory(
        IOptions<CassandraOptions> cassandraOptions,
        ILogger<CassandraSessionFactory> logger)
    {
        _logger = logger;
        var options = cassandraOptions.Value;
        
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetry(
                2,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt) * 5),
                (exception, timespan, context) =>
                {
                    _logger.LogInformation($"{timespan}, {context.Count}, {exception}");
                });

        retryPolicy.Execute(
            () =>
            {
                var cluster = Cluster
                    .Builder()
                    .WithCredentials(options.UserName, options.Password)
                    .WithPort(options.Port)
                    .AddContactPoint(options.Address)
                    .Build();

                var session = cluster.Connect();
                session.CreateKeyspaceIfNotExists(options.KeySpace);
                session.ChangeKeyspace(options.KeySpace);

                _session = new Lazy<ISession>(() => session);
            });
    }

    public ISession GetSession()
    {
        return _session.Value;
    }
}
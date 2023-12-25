using Cassandra;
using Domain.Options;
using Microsoft.Extensions.Options;

namespace Domain.Factories;

public class CassandraSessionFactory : ICassandraSessionFactory
{
    private readonly Lazy<ISession> _session;

    public CassandraSessionFactory(IOptions<CassandraOptions> cassandraOptions)
    {
        var options = cassandraOptions.Value;

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
    }

    public virtual ISession GetSession()
    {
        return _session.Value;
    }
}
using Cassandra;

namespace Domain.Migrations
{
    public class CassandraMigrationService : IDisposable
    {
        private readonly Cluster _cluster;
        private readonly ISession _session;

        public CassandraMigrationService(ICassandraOptions options)
        {
            var cluster = Cluster
                .Builder()
                .WithCredentials(options.UserName, options.Password)
                .WithDefaultKeyspace(options.KeySpace)
                .WithPort(options.Port)
                .AddContactPoint(options.Address)
                .Build();

            _cluster = cluster;
            _session = _cluster.Connect();
        }

        public void ApplyMigrations()
        {
            MigrateToV1();
        }

        private void MigrateToV1()
        {
            _session.Execute("DROP KEYSPACE my_keyspace;");
            _session.Execute("DROP TABLE IF EXISTS my_keyspace.User;");
            _session.Execute("DROP TABLE IF EXISTS my_keyspace.PasswordStorage;");
            _session.Execute("CREATE KEYSPACE IF NOT EXISTS my_keyspace WITH replication = {'class': 'SimpleStrategy','replication_factor': 1};");
            _session.Execute("CREATE TABLE IF NOT EXISTS my_keyspace.User (login TEXT PRIMARY KEY, password TEXT);");
            _session.Execute("CREATE TABLE IF NOT EXISTS my_keyspace.PasswordStorage (login TEXT, resourceName TEXT, resourcePassword TEXT, PRIMARY KEY (login, resourceName));");
        }

        public void Dispose()
        {
            _session.Dispose();
            _cluster.Dispose();
        }
    }
}
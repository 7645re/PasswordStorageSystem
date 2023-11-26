using System;
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
            MigrateToV2();
        }

        private void MigrateToV1()
        {
            _session.Execute("CREATE TABLE IF NOT EXISTS my_keyspace.User (login TEXT PRIMARY KEY, password TEXT);");
            _session.Execute("CREATE TABLE IF NOT EXISTS my_keyspace.ResourceCredential (login TEXT, resource_name TEXT, resource_login TEXT, resource_password TEXT, last_update TIMESTAMP, PRIMARY KEY (login, resource_name, resource_login));");
            _session.Execute("CREATE INDEX IF NOT EXISTS last_updated_index ON my_keyspace.ResourceCredential (last_update);");
        }

        private void MigrateToV2()
        {
            
        }

        public void Dispose()
        {
            _session.Dispose();
            _cluster.Dispose();
        }
    }
}
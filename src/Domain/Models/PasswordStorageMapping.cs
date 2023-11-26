using Cassandra.Mapping;

namespace Domain.Models;

public class PasswordStorageMapping : Mappings
{
    public PasswordStorageMapping()
    {
        For<ResourceCredential>()
            .TableName("ResourceCredential")
            .PartitionKey(ps => ps.Login, ps => ps.ResourceName, ps => ps.ResourceLogin)
            .Column(ps => ps.Login, cm => cm.WithName("login"))
            .Column(ps => ps.ResourceName, cm => cm.WithName("resource_name"))
            .Column(ps => ps.ResourcePassword, cm => cm.WithName("resource_password"))
            .Column(ps => ps.ResourceLogin, cm => cm.WithName("resource_login"))
            .Column(ps => ps.LastUpdate, cm => cm.WithName("last_update"))
            .KeyspaceName("my_keyspace");
    }
}
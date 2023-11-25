using Cassandra.Mapping;

namespace Domain.Models;

public class PasswordStorageMapping : Mappings
{
    public PasswordStorageMapping()
    {
        For<PasswordStorage>()
            .TableName("PasswordStorage")
            .PartitionKey(ps => ps.Login, ps => ps.ResourceName)
            .Column(ps => ps.Login, cm => cm.WithName("login"))
            .Column(ps => ps.ResourceName, cm => cm.WithName("resourceName"))
            .Column(ps => ps.ResourcePassword, cm => cm.WithName("resourcePassword"))
            .KeyspaceName("my_keyspace");
    }
}
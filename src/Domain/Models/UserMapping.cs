using Cassandra.Mapping;

namespace Domain.Models;

public class UserMapping : Mappings
{
    public UserMapping()
    {
        For<User>()
            .TableName("User")
            .PartitionKey(u => u.Login)
            .Column(u => u.Login, cm => cm.WithName("login"))
            .Column(u => u.Password, cm => cm.WithName("password"))
            .KeyspaceName("my_keyspace");
    }
}
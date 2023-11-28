using Cassandra.Mapping.Attributes;

namespace Domain.Models;

[Table(Name = "credentials_history", Keyspace = "my_keyspace")]
public class CredentialHistoryItemEntity
{
    [PartitionKey] 
    [Column("user_login")]
    public string UserLogin { get; set; }

    [ClusteringKey(1)]
    [Column("resource_name")]
    public string ResourceName { get; set; }

    [ClusteringKey(2)]
    [Column("resource_login")]
    public string ResourceLogin { get; set; }

    [ClusteringKey(3)]
    [Column("resource_password")]
    public string ResourcePassword { get; set; }

    [Column("change_at")]
    public DateTimeOffset ChangeAt { get; set; }
}
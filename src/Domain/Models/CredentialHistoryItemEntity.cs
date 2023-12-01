using Cassandra.Mapping;
using Cassandra.Mapping.Attributes;

namespace Domain.Models;

[Table(Name = "credentials_history", Keyspace = "my_keyspace")]
public class CredentialHistoryItemEntity
{
    [PartitionKey] 
    [Column("user_login")]
    public string UserLogin { get; set; } = string.Empty;

    [ClusteringKey(1, SortOrder.Ascending)]
    [Column("resource_name")]
    public string ResourceName { get; set; } = string.Empty;

    [ClusteringKey(2, SortOrder.Ascending)]
    [Column("resource_login")]
    public string ResourceLogin { get; set; } = string.Empty;

    [ClusteringKey(3, SortOrder.Ascending)]
    [Column("resource_password")]
    public string ResourcePassword { get; set; } = string.Empty;

    [ClusteringKey(4, SortOrder.Descending)]
    [Column("change_at")]
    public DateTimeOffset ChangeAt { get; set; }
}
using Cassandra.Mapping;
using Cassandra.Mapping.Attributes;

namespace Domain.Models;

[Table("credentials_by_password", Keyspace = "password_storage_system")]
public class CredentialByPasswordEntity
{
    [PartitionKey]
    [Column("resource_password")]
    public string ResourcePassword { get; set; } = string.Empty;
    
    [ClusteringKey(0, SortOrder.Descending)]
    [Column("user_login")]
    public string UserLogin { get; set; } = string.Empty;

    [ClusteringKey(1, SortOrder.Descending)]
    [Column("resource_name")]
    public string ResourceName { get; set; } = string.Empty;
    
    [ClusteringKey(1, SortOrder.Descending)]
    [Column("resource_login")]
    public string ResourceLogin { get; set; } = string.Empty;
}
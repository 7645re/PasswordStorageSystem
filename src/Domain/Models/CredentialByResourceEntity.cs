using Cassandra.Mapping.Attributes;

namespace Domain.Models;

[Table("credentials_by_resource", Keyspace = "password_storage_system")]
public class CredentialByResourceEntity
{
    [PartitionKey]
    [Column("user_login")]
    public string UserLogin { get; set; } = string.Empty;

    [ClusteringKey(1)]
    [Column("resource_name")]
    public string ResourceName { get; set; } = string.Empty;

    [ClusteringKey(2)]
    [Column("resource_login")]
    public string ResourceLogin { get; set; } = string.Empty;
}
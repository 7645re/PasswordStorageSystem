using Cassandra.Mapping;
using Cassandra.Mapping.Attributes;
using Domain.Enums;

namespace Domain.Models;

[Table("credentials_by_security_level", Keyspace = "password_storage_system")]
public class CredentialBySecurityLevelEntity
{
    [PartitionKey(0)]
    [Column("password_security_level")]
    public int PasswordSecurityLevel { get; set; }
    
    [PartitionKey(1)]
    [Column("user_login")]
    public string UserLogin { get; set; } = string.Empty;
    
    [ClusteringKey(0, SortOrder.Descending)]
    [Column("resource_name")]
    public string ResourceName { get; set; } = string.Empty;

    [ClusteringKey(1, SortOrder.Descending)]
    [Column("resource_login")]
    public string ResourceLogin { get; set; } = string.Empty;
}
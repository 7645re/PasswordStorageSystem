using Cassandra.Mapping;
using Cassandra.Mapping.Attributes;
using Domain.Enums;

namespace Domain.Models;

[Table("credentials", Keyspace = "password_storage_system")]
public class CredentialEntity
{
    [PartitionKey]
    [Column("user_login")]
    public string UserLogin { get; set; } = string.Empty;

    [ClusteringKey(0, SortOrder.Descending)]
    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }
    
    [ClusteringKey(1, SortOrder.Descending)]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("resource_name")]
    public string ResourceName { get; set; } = string.Empty;

    [Column("resource_login")]
    public string ResourceLogin { get; set; } = string.Empty;

    [Column("resource_password")]
    public string ResourcePassword { get; set; } = string.Empty;

    [Column("password_security_level", Type = typeof(int))]
    public PasswordSecurityLevel PasswordSecurityLevel { get; set; }

    [Column("changed_at")]
    public DateTimeOffset? ChangedAt { get; set; }
}
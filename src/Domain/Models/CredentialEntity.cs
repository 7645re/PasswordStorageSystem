using Cassandra.Mapping;
using Cassandra.Mapping.Attributes;
using Domain.Enums;

namespace Domain.Models;

[Table("credentials", Keyspace = "my_keyspace")]
public class CredentialEntity
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

    [SecondaryIndex]
    [Column("resource_password")]
    public string ResourcePassword { get; set; } = string.Empty;
    
    [SecondaryIndex]
    [Column("password_security_level", Type = typeof(int))]
    public PasswordSecurityLevel PasswordSecurityLevel { get; set; }
    
    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }
    
    [Column("changed_at")]
    public DateTimeOffset? ChangeAt { get; set; }
}
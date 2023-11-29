using Cassandra.Mapping.Attributes;
using Domain.Enums;

namespace Domain.Models;

[Table("credentials", Keyspace = "my_keyspace")]
public class CredentialEntity
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

    [SecondaryIndex]
    [Column("resource_password")]
    public string ResourcePassword { get; set; }
    
    [SecondaryIndex]
    [Column("password_security_level", Type = typeof(int))]
    public PasswordSecurityLevel PasswordSecurityLevel { get; set; }
    
    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }
    
    [Column("changed_at")]
    public DateTimeOffset? ChangeAt { get; set; }
}
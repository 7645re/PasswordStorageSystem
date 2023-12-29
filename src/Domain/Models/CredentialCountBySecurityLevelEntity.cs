using Cassandra.Mapping.Attributes;
using Domain.Enums;

namespace Domain.Models;

[Table("credentials_count_by_security_level", Keyspace = "password_storage_system")]
public class CredentialCountBySecurityLevelEntity
{
    [PartitionKey]
    [Column("user_login")]
    public string UserLogin { get; set; } = string.Empty;

    [PartitionKey(1)]
    [Column("password_security_level", Type = typeof(int))]
    public PasswordSecurityLevel PasswordSecurityLevel { get; set; }
    
    [Column("count")]
    [Counter]
    public long Count { get; set; }
}
using Cassandra.Mapping.Attributes;

namespace Domain.Models;

[Table("credentials_count_by_security_level", Keyspace = "password_storage_system")]
public class CredentialCountBySecurityLevelEntity
{
    [PartitionKey]
    [Column("user_login")]
    public string UserLogin { get; set; } = string.Empty;

    [PartitionKey(1)]
    [Column("password_security_level")]
    public int PasswordSecurityLevel { get; set; }

    [Column("count")]
    public int Count { get; set; }
}
using Cassandra.Mapping.Attributes;

namespace Domain.Models;

[Table("users", Keyspace = "password_storage_system")]
public class UserEntity
{
    [PartitionKey]
    [Column("login")]
    public string Login { get; set; } = string.Empty;

    [Column("password")]
    public string Password { get; set; } = string.Empty;

    [Column("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [Column("access_token_expire")]
    public DateTimeOffset AccessTokenExpire { get; set; }
}
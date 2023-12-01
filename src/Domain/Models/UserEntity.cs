using Cassandra.Mapping.Attributes;

namespace Domain.Models;

[Table("users", Keyspace = "my_keyspace")]
public class UserEntity
{
    [PartitionKey]
    [Column("login")]
    public string Login { get; set; } = string.Empty;
        
    [Column("password")]
    public string Password { get; set; } = string.Empty;

    [Column("jwt_token")] 
    public string Token { get; set; } = string.Empty;

    [Column("token_expire")]
    public DateTimeOffset TokenExpire { get; set; }
}
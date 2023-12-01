using Cassandra.Mapping.Attributes;

namespace Domain.Models;

[Table("tokens_black_list", Keyspace = "my_keyspace")]
public class TokenBlackListEntity
{
    [PartitionKey]
    [Column("jwt_token")]
    public string Token { get; set; } = string.Empty;
    
    [Column("expire")]
    public DateTimeOffset Expire { get; set; }
}
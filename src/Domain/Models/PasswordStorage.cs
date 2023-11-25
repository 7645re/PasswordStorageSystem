using Cassandra.Mapping.Attributes;

namespace Domain.Models;

[Table("PasswordStorage", Keyspace = "my_keyspace")]
public class PasswordStorage
{
    [PartitionKey(1)]
    [Column("login")]
    public string Login { get; set; }
        
    [PartitionKey(2)]
    [Column("resourceName")]
    public string ResourceName { get; set; }
        
    [Column("resourcePassword")]
    public string ResourcePassword { get; set; }
}
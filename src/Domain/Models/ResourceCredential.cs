using System;
using Cassandra.Mapping.Attributes;

namespace Domain.Models;

[Table("ResourceCredential", Keyspace = "my_keyspace")]
public class ResourceCredential
{
    [Column("last_update")]
    public DateTimeOffset LastUpdate { get; set; }
    
    [PartitionKey(1)]
    [Column("login")]
    public string Login { get; set; }

    [PartitionKey(3)]
    [Column("resource_login")]
    public string ResourceLogin { get; set; }
    
    [PartitionKey(2)]
    [Column("resource_name")]
    public string ResourceName { get; set; }

    [Column("resource_password")]
    public string ResourcePassword { get; set; }
}
using Cassandra.Mapping.Attributes;

namespace Domain.Models;

[Table("User", Keyspace = "my_keyspace")]
public class User
{
    [PartitionKey]
    [Column("login")]
    public string Login { get; set; }
        
    [Column("password")]
    public string Password { get; set; }
    
    public User(string login, string password)
    {
        Login = login;
        Password = password;
    }
}
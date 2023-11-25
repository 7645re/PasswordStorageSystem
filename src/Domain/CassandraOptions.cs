namespace Domain;

public class CassandraOptions : ICassandraOptions
{
    public string Address { get; set; } = null!;
    public int Port { get; set; }
    public string KeySpace { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
}
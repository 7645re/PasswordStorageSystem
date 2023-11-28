namespace Domain.Options;

public class CassandraOptions
{
    public string Address { get; set; } = string.Empty;
    public int Port { get; set; }
    public string KeySpace { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
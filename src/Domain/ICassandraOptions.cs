namespace Domain;

public interface ICassandraOptions
{
    public string Address { get; set; }
    public int Port { get; set; }
    public string KeySpace { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}
using Cassandra;

namespace Domain.Factories;

public interface ICassandraSessionFactory
{
    ISession GetSession();
}
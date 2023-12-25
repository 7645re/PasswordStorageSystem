using Cassandra.Mapping;
using Cassandra.Mapping.Attributes;

namespace Domain.Models;

[Table(Name = "credentials_history", Keyspace = "password_storage_system")]
public class CredentialHistoryItemEntity
{
    [PartitionKey]
    [Column("credential_id")]
    public Guid CredentialId { get; set; }

    [ClusteringKey(0, SortOrder.Descending)]
    [Column("changed_at")]
    public DateTimeOffset ChangedAt { get; set; }

    [Column("resource_password")] public string ResourcePassword { get; set; } = string.Empty;
}
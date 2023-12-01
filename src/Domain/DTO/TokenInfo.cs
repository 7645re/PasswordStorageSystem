namespace Domain.DTO;

public record TokenInfo(
    string Token,
    DateTimeOffset Expire);
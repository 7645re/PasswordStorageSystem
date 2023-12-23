namespace WebAPI.Filters;

public record ExceptionsResponse
{
    public IReadOnlyCollection<ExceptionDetail> Exceptions { get; init; } = Array.Empty<ExceptionDetail>();
}
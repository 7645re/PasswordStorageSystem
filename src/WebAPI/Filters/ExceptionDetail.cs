namespace WebAPI.Filters;

public class ExceptionDetail
{
    public string Error { get; init; } = string.Empty;

    public string Parameter { get; init; } = string.Empty;

    public Guid Code { get; init; }

    public string Message { get; init; } = string.Empty;
}
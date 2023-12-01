namespace Domain.Services;

public class OperationResultBase
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
}

public class OperationResult<T> : OperationResultBase
{
    public T? Result { get; set; }
}

public class OperationResult : OperationResultBase
{
}
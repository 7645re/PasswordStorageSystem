namespace Domain.Validators;

public record ValidateResult(bool IsSuccess, string? ErrorMessage = null);
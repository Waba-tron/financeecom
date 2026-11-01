using FluentValidation.Results;

namespace EcomBackend.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when validation fails.
/// </summary>
public class ValidationFailedException : AppException
{
    public IReadOnlyList<ValidationFailure> ValidationFailures { get; }

    public ValidationFailedException(IEnumerable<ValidationFailure> failures)
        : base("One or more validation failures have occurred.", "VALIDATION_FAILED", 422)
    {
        ValidationFailures = failures.ToList().AsReadOnly();
    }
}

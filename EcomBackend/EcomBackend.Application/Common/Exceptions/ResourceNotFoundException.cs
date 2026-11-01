namespace EcomBackend.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when a requested resource is not found.
/// </summary>
public class ResourceNotFoundException : AppException
{
    public ResourceNotFoundException(string resourceName, object? identifier = null)
        : base(
            identifier is not null
                ? $"{resourceName} with identifier '{identifier}' was not found."
                : $"{resourceName} was not found.",
            "RESOURCE_NOT_FOUND",
            404)
    {
    }
}

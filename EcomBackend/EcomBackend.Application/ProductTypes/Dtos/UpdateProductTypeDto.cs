namespace EcomBackend.Application.ProductTypes.Dtos;

public record UpdateProductTypeDto(
    string Name,
    string? Description,
    bool IsActive
);

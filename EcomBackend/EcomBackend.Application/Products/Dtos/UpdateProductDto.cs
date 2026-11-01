namespace EcomBackend.Application.Products.Dtos;

public record UpdateProductDto(
    string Name,
    string Description,
    decimal Price,
    int ProductTypeId
);

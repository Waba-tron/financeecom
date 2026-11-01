using EcomBackend.Application.ProductTypes;
using MediatR;
using System.Xml.Linq;

public record UpdateProductTypeCommand(
    int Id,
    string Name,
    string? Description,
    bool IsActive
) : IRequest<ProductTypeDto?>;

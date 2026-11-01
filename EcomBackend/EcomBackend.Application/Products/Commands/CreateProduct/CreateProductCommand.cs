using MediatR;

namespace EcomBackend.Application.Products.Commands.CreateProduct;

public record CreateProductCommand(string Name, string Description, decimal Price, int ProductTypeId)
    : IRequest<ProductDto>;

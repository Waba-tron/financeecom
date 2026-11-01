using MediatR;

namespace EcomBackend.Application.ProductTypes.Commands.CreateProductType;

public record CreateProductTypeCommand(string Name, string? Description) : IRequest<ProductTypeDto>;

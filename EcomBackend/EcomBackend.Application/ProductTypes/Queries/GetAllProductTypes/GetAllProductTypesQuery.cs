using MediatR;

namespace EcomBackend.Application.ProductTypes.Queries.GetAllProductTypes;

public record GetAllProductTypesQuery : IRequest<List<ProductTypeDto>>;

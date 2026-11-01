using EcomBackend.Application.Common.Models;
using MediatR;

namespace EcomBackend.Application.Products.Queries.GetAllProducts;

public record GetAllProductsQuery(
    int? ProductTypeId = null,
    string? Search = null,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedResult<ProductDto>>;

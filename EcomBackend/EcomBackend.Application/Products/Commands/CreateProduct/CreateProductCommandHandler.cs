using EcomBackend.Application.Common.Interfaces;
using EcomBackend.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcomBackend.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IApplicationDbContext _context;

    public CreateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var productType = await _context.ProductTypes
            .FirstOrDefaultAsync(t => t.Id == request.ProductTypeId, cancellationToken)
            ?? throw new InvalidOperationException($"ProductType {request.ProductTypeId} not found.");

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            CreatedDate = DateTime.UtcNow,
            ProductTypeId = productType.Id,
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CreatedDate = product.CreatedDate,
            ProductTypeId = productType.Id,
            ProductTypeName = productType.Name
        };
    }
}

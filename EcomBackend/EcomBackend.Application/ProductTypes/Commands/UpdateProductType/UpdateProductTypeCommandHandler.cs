using EcomBackend.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcomBackend.Application.ProductTypes.Commands.UpdateProductType;

public class UpdateProductTypeCommandHandler : IRequestHandler<UpdateProductTypeCommand, ProductTypeDto?>
{
    private readonly IApplicationDbContext _context;

    public UpdateProductTypeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductTypeDto?> Handle(UpdateProductTypeCommand request, CancellationToken cancellationToken)
    {
        var productType = await _context.ProductTypes
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (productType == null)
            return null;

        var name = request.Name.Trim();

        var duplicate = await _context.ProductTypes
            .AnyAsync(t => t.Id != request.Id && t.Name == name, cancellationToken);

        if (duplicate)
            throw new InvalidOperationException($"ProductType '{name}' already exists.");

        productType.Name = name;
        productType.Description = request.Description;
        productType.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

        return new ProductTypeDto
        {
            Id = productType.Id,
            Name = productType.Name,
            Description = productType.Description
        };
    }
}

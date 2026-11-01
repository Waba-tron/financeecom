using EcomBackend.Application.Common.Interfaces;
using EcomBackend.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcomBackend.Application.ProductTypes.Commands.CreateProductType;

public class CreateProductTypeCommandHandler : IRequestHandler<CreateProductTypeCommand, ProductTypeDto>
{
    private readonly IApplicationDbContext _context;

    public CreateProductTypeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductTypeDto> Handle(CreateProductTypeCommand request, CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();

        var exists = await _context.ProductTypes.AnyAsync(t => t.Name == name, cancellationToken);
        if (exists)
            throw new InvalidOperationException($"ProductType '{name}' already exists.");

        var productType = new ProductType
        {
            Name = name,
            Description = request.Description,
            IsActive = true
        };

        _context.ProductTypes.Add(productType);
        await _context.SaveChangesAsync(cancellationToken);

        return new ProductTypeDto
        {
            Id = productType.Id,
            Name = productType.Name,
            Description = productType.Description
        };
    }
}

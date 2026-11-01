# Scaffold a Clean Architecture CRUD Web API

Create a new .NET 10 Clean Architecture solution with CQRS (MediatR) and EF Core (SQL Server LocalDB), exposing CRUD endpoints for a `Product` entity.

## Inputs

- **ProjectName**: `{{PROJECT_NAME}}` — the base name for the solution and all projects. Replace every occurrence of `CrudApplication` below with this value.
- **RootPath** (optional): folder in which to create the solution. Defaults to a new folder named `{{PROJECT_NAME}}` in the current working directory.

If `{{PROJECT_NAME}}` is not provided, ask the user for it before generating any files.

## Goal

Produce a working, buildable solution with this structure:

```
{{PROJECT_NAME}}.sln
{{PROJECT_NAME}}.Domain/
{{PROJECT_NAME}}.Application/
{{PROJECT_NAME}}.Infrastructure/
{{PROJECT_NAME}}.API/
```

## Setup Commands

Run from the desired root directory (PowerShell):

```powershell
$name = "{{PROJECT_NAME}}"

dotnet new sln -n $name

dotnet new classlib   -n "$name.Domain"         -f net10.0
dotnet new classlib   -n "$name.Application"    -f net10.0
dotnet new classlib   -n "$name.Infrastructure" -f net10.0
dotnet new webapi     -n "$name.API"            -f net10.0 --use-controllers

# Remove default template files
Remove-Item "$name.Domain\Class1.cs" -ErrorAction SilentlyContinue
Remove-Item "$name.Application\Class1.cs" -ErrorAction SilentlyContinue
Remove-Item "$name.Infrastructure\Class1.cs" -ErrorAction SilentlyContinue
Remove-Item "$name.API\WeatherForecast.cs" -ErrorAction SilentlyContinue
Remove-Item "$name.API\Controllers\WeatherForecastController.cs" -ErrorAction SilentlyContinue

dotnet sln add "$name.Domain\$name.Domain.csproj"
dotnet sln add "$name.Application\$name.Application.csproj"
dotnet sln add "$name.Infrastructure\$name.Infrastructure.csproj"
dotnet sln add "$name.API\$name.API.csproj"

# Project references
dotnet add "$name.Application\$name.Application.csproj"       reference "$name.Domain\$name.Domain.csproj"
dotnet add "$name.Infrastructure\$name.Infrastructure.csproj" reference "$name.Domain\$name.Domain.csproj" "$name.Application\$name.Application.csproj"
dotnet add "$name.API\$name.API.csproj"                       reference "$name.Application\$name.Application.csproj" "$name.Infrastructure\$name.Infrastructure.csproj"

# NuGet packages
dotnet add "$name.Application\$name.Application.csproj" package MediatR --version 12.4.1
dotnet add "$name.Application\$name.Application.csproj" package Microsoft.EntityFrameworkCore --version 9.0.0

dotnet add "$name.Infrastructure\$name.Infrastructure.csproj" package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.0
dotnet add "$name.Infrastructure\$name.Infrastructure.csproj" package Microsoft.EntityFrameworkCore.Design   --version 9.0.0
dotnet add "$name.Infrastructure\$name.Infrastructure.csproj" package Microsoft.Extensions.DependencyInjection.Abstractions --version 9.0.0

dotnet add "$name.API\$name.API.csproj" package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.0
dotnet add "$name.API\$name.API.csproj" package Microsoft.EntityFrameworkCore.Design   --version 9.0.0
dotnet add "$name.API\$name.API.csproj" package Swashbuckle.AspNetCore                 --version 10.2.1
```

All csproj files should have `<Nullable>enable</Nullable>` and `<ImplicitUsings>enable</ImplicitUsings>`.

## Files to Create

Replace every `CrudApplication` below with `{{PROJECT_NAME}}`.

### `{{PROJECT_NAME}}.Domain/Entities/Product.cs`

```csharp
namespace CrudApplication.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
```

### `{{PROJECT_NAME}}.Application/Common/Interfaces/IApplicationDbContext.cs`

```csharp
using CrudApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CrudApplication.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

### `{{PROJECT_NAME}}.Application/DependencyInjection.cs`

```csharp
using Microsoft.Extensions.DependencyInjection;

namespace CrudApplication.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}
```

### `{{PROJECT_NAME}}.Application/Products/ProductDto.cs`

```csharp
namespace CrudApplication.Application.Products;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime CreatedDate { get; set; }
}
```

### `{{PROJECT_NAME}}.Application/Products/Commands/CreateProduct/CreateProductCommand.cs`

```csharp
using MediatR;

namespace CrudApplication.Application.Products.Commands.CreateProduct;

public record CreateProductCommand(string Name, string Description, decimal Price)
    : IRequest<ProductDto>;
```

### `{{PROJECT_NAME}}.Application/Products/Commands/CreateProduct/CreateProductCommandHandler.cs`

```csharp
using CrudApplication.Application.Common.Interfaces;
using CrudApplication.Domain.Entities;
using MediatR;

namespace CrudApplication.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IApplicationDbContext _context;

    public CreateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            CreatedDate = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CreatedDate = product.CreatedDate
        };
    }
}
```

### `{{PROJECT_NAME}}.Application/Products/Commands/DeleteProduct/DeleteProductCommand.cs`

```csharp
using MediatR;

namespace CrudApplication.Application.Products.Commands.DeleteProduct;

public record DeleteProductCommand(int Id) : IRequest<bool>;
```

### `{{PROJECT_NAME}}.Application/Products/Commands/DeleteProduct/DeleteProductCommandHandler.cs`

```csharp
using CrudApplication.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CrudApplication.Application.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product == null)
            return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
```

### `{{PROJECT_NAME}}.Application/Products/Queries/GetAllProducts/GetAllProductsQuery.cs`

```csharp
using MediatR;

namespace CrudApplication.Application.Products.Queries.GetAllProducts;

public record GetAllProductsQuery : IRequest<List<ProductDto>>;
```

### `{{PROJECT_NAME}}.Application/Products/Queries/GetAllProducts/GetAllProductsQueryHandler.cs`

```csharp
using CrudApplication.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CrudApplication.Application.Products.Queries.GetAllProducts;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<ProductDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Products
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CreatedDate = p.CreatedDate
            })
            .ToListAsync(cancellationToken);
    }
}
```

### `{{PROJECT_NAME}}.Application/Products/Queries/GetProductById/GetProductByIdQuery.cs`

```csharp
using MediatR;

namespace CrudApplication.Application.Products.Queries.GetProductById;

public record GetProductByIdQuery(int Id) : IRequest<ProductDto?>;
```

### `{{PROJECT_NAME}}.Application/Products/Queries/GetProductById/GetProductByIdQueryHandler.cs`

```csharp
using CrudApplication.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CrudApplication.Application.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IApplicationDbContext _context;

    public GetProductByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .Where(p => p.Id == request.Id)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CreatedDate = p.CreatedDate
            })
            .FirstOrDefaultAsync(cancellationToken);

        return product;
    }
}
```

### `{{PROJECT_NAME}}.Infrastructure/Data/ApplicationDbContext.cs`

```csharp
using CrudApplication.Application.Common.Interfaces;
using CrudApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CrudApplication.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Price).HasPrecision(18, 2);
        });
    }
}
```

### `{{PROJECT_NAME}}.Infrastructure/DependencyInjection.cs`

```csharp
using CrudApplication.Application.Common.Interfaces;
using CrudApplication.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CrudApplication.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}
```

### `{{PROJECT_NAME}}.API/Controllers/ProductsController.cs`

```csharp
using CrudApplication.Application.Products.Commands.CreateProduct;
using CrudApplication.Application.Products.Commands.DeleteProduct;
using CrudApplication.Application.Products.Queries.GetAllProducts;
using CrudApplication.Application.Products.Queries.GetProductById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CrudApplication.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(id));
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllProductsQuery());
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteProductCommand(id));
        return result ? NoContent() : NotFound();
    }
}
```

### `{{PROJECT_NAME}}.API/Program.cs`

```csharp
using CrudApplication.Application;
using CrudApplication.Infrastructure;
using CrudApplication.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("DefaultConnection")!);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### `{{PROJECT_NAME}}.API/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database={{PROJECT_NAME}};Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

## Finish

1. Run `dotnet build` from the solution root and confirm the build succeeds.
2. Run `dotnet run --project "{{PROJECT_NAME}}.API"` and browse to `/swagger` to verify the CRUD endpoints for Products.

## Rules for the agent executing this prompt

- Replace **every** occurrence of `CrudApplication` in namespaces, using directives, and file paths with `{{PROJECT_NAME}}`.
- Do not add extra files, controllers, or entities beyond `Product`.
- Keep folder layout exactly as shown (feature-folder CQRS under `Products/Commands` and `Products/Queries`).
- Target framework must be `net10.0` with nullable and implicit usings enabled.
- Do not commit any `bin/` or `obj/` folders.

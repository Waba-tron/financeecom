using EcomBackend.Domain.Entities;
using EcomBackend.Domain.Payments;
using Microsoft.EntityFrameworkCore;

namespace EcomBackend.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }
    DbSet<ProductType> ProductTypes { get; }
    DbSet<User> Users { get; }
    DbSet<Cart> Carts { get; }
    DbSet<CartItem> CartItems { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<Payment> Payments { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

using EcomBackend.Application.Common.Interfaces;
using EcomBackend.Domain.Entities;
using EcomBackend.Domain.Payments;
using Microsoft.EntityFrameworkCore;

namespace EcomBackend.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<ProductType> ProductTypes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.DisplayName).HasMaxLength(100);
            entity.Property(e => e.Role).IsRequired().HasMaxLength(50);

            // Note: the admin user is seeded at runtime in Program.cs so the
            // password can be hashed via IPasswordHasher (BCrypt). EF Core's
            // HasData requires compile-time constants, so it can't be used here.
        });

        modelBuilder.Entity<ProductType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasData(
                new ProductType { Id = 1, Name = "Loan",          IsActive = true, Description = "Personal or business loans" },
                new ProductType { Id = 2, Name = "Credit Card",   IsActive = true, Description = "Credit cards with various tiers" },
                new ProductType { Id = 3, Name = "Insurance",     IsActive = true, Description = "Life, health, and general insurance" },
                new ProductType { Id = 4, Name = "Mutual Fund",   IsActive = true, Description = "Managed investment funds" },
                new ProductType { Id = 5, Name = "Savings",       IsActive = true, Description = "Savings accounts" },
                new ProductType { Id = 6, Name = "Fixed Deposit", IsActive = true, Description = "Fixed-term deposits" }
            );
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Price).HasPrecision(18, 2);

            entity.HasOne(e => e.ProductType)
                  .WithMany(t => t.Products)
                  .HasForeignKey(e => e.ProductTypeId)
                  .OnDelete(DeleteBehavior.Restrict);

            var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            entity.HasData(
                // Loans (ProductTypeId = 1)
                new Product { Id = 1, ProductTypeId = 1, Name = "Personal Loan",           Description = "Unsecured personal loan up to $50,000",       Price = 199.00m,  CreatedDate = seedDate },
                new Product { Id = 2, ProductTypeId = 1, Name = "Home Loan",               Description = "Mortgage with competitive fixed rates",       Price = 499.00m,  CreatedDate = seedDate },
                new Product { Id = 3, ProductTypeId = 1, Name = "Auto Loan",               Description = "Financing for new and used vehicles",         Price = 149.00m,  CreatedDate = seedDate },

                // Credit Cards (ProductTypeId = 2)
                new Product { Id = 4, ProductTypeId = 2, Name = "Platinum Credit Card",    Description = "Premium card with travel rewards",            Price = 250.00m,  CreatedDate = seedDate },
                new Product { Id = 5, ProductTypeId = 2, Name = "Cashback Credit Card",    Description = "2% cashback on all purchases",                Price = 0.00m,    CreatedDate = seedDate },

                // Insurance (ProductTypeId = 3)
                new Product { Id = 6, ProductTypeId = 3, Name = "Term Life Insurance",     Description = "20-year term life cover",                     Price = 45.00m,   CreatedDate = seedDate },
                new Product { Id = 7, ProductTypeId = 3, Name = "Health Insurance",        Description = "Comprehensive family health plan",            Price = 320.00m,  CreatedDate = seedDate },

                // Mutual Funds (ProductTypeId = 4)
                new Product { Id = 8, ProductTypeId = 4, Name = "Equity Growth Fund",      Description = "Diversified large-cap equity fund",           Price = 100.00m,  CreatedDate = seedDate },
                new Product { Id = 9, ProductTypeId = 4, Name = "Balanced Fund",           Description = "60/40 equity and debt allocation",            Price = 100.00m,  CreatedDate = seedDate },

                // Savings (ProductTypeId = 5)
                new Product { Id = 10, ProductTypeId = 5, Name = "High-Yield Savings",     Description = "4.5% APY savings account",                    Price = 0.00m,    CreatedDate = seedDate },

                // Fixed Deposit (ProductTypeId = 6)
                new Product { Id = 11, ProductTypeId = 6, Name = "12-Month Fixed Deposit", Description = "Locked-in 5.25% annual return",               Price = 1000.00m, CreatedDate = seedDate },
                new Product { Id = 12, ProductTypeId = 6, Name = "24-Month Fixed Deposit", Description = "Locked-in 5.75% annual return",               Price = 1000.00m, CreatedDate = seedDate }
            );
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId).IsUnique();

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.HasIndex(e => new { e.CartId, e.ProductId }).IsUnique();

            entity.HasOne(e => e.Cart)
                  .WithMany(c => c.Items)
                  .HasForeignKey(e => e.CartId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Product)
                  .WithMany()
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Total).HasPrecision(18, 2);
            entity.Property(e => e.Status).HasConversion<int>();

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);

            entity.HasOne(e => e.Order)
                  .WithMany(o => o.Items)
                  .HasForeignKey(e => e.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SessionId).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.SessionId).IsUnique();
            entity.Property(e => e.Currency).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Status).HasConversion<int>();
        });
    }
}

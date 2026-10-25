using CleanArchitectureProductsAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureProductsAPI.Infrastructure.Persistence;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Picks up ProductConfiguration (and any future configurations).
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductDbContext).Assembly);
    }
}

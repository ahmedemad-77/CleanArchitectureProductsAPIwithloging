using CleanArchitectureProductsAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitectureProductsAPI.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Price)
            .HasPrecision(18, 2);

        // Sample data, inserted by the migration.
        builder.HasData(
            new { Id = 1, Name = "Laptop",   Price = 999.99m },
            new { Id = 2, Name = "Mouse",    Price = 19.99m },
            new { Id = 3, Name = "Keyboard", Price = 49.50m });
    }
}

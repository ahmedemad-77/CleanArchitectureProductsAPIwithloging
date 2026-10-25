using CleanArchitectureProductsAPI.Domain.Entities;

namespace CleanArchitectureProductsAPI.Application.Abstractions;

// Application defines WHAT it needs; Infrastructure decides HOW (EF Core).
public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(CancellationToken cancellationToken);
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(Product product, CancellationToken cancellationToken);
}

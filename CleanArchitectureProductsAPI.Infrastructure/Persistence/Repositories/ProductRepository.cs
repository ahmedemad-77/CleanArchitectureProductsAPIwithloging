using CleanArchitectureProductsAPI.Application.Abstractions;
using CleanArchitectureProductsAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureProductsAPI.Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductDbContext _context;

    public ProductRepository(ProductDbContext context)
    {
        _context = context;
    }

    public Task<List<Product>> GetAllAsync(CancellationToken cancellationToken)
    {
        // AsNoTracking: we only read, so EF doesn't need to track changes.
        return _context.Products
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        _context.Products.Add(product);                     // only marks it as "to insert"
        await _context.SaveChangesAsync(cancellationToken); // actually writes to SQL Server
    }
}

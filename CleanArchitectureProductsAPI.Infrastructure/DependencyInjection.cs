using CleanArchitectureProductsAPI.Application.Abstractions;
using CleanArchitectureProductsAPI.Infrastructure.Persistence;
using CleanArchitectureProductsAPI.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureProductsAPI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not found.");

        services.AddDbContext<ProductDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Scoped = one instance per HTTP request (same lifetime as the DbContext).
        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}

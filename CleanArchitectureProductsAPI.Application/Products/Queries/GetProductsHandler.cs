using CleanArchitectureProductsAPI.Application.Abstractions;
using CleanArchitectureProductsAPI.Application.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureProductsAPI.Application.Products.Queries;

public class GetProductsHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    private readonly IProductRepository _repository;
    private readonly ILogger<GetProductsHandler> _logger;

    public GetProductsHandler(IProductRepository repository, ILogger<GetProductsHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<ProductDto>> Handle(
        GetProductsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving the product list");

        var products = await _repository.GetAllAsync(cancellationToken);

        _logger.LogInformation("Retrieved {ProductCount} products", products.Count);

        return products
            .Select(p => new ProductDto(p.Id, p.Name, p.Price))
            .ToList();
    }
}

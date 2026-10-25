using CleanArchitectureProductsAPI.Application.Abstractions;
using CleanArchitectureProductsAPI.Application.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureProductsAPI.Application.Products.Queries;

public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductRepository _repository;
    private readonly ILogger<GetProductByIdHandler> _logger;

    public GetProductByIdHandler(IProductRepository repository, ILogger<GetProductByIdHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ProductDto?> Handle(
        GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Product {ProductId} was requested", request.Id);

        var product = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (product is null)
        {
            // Expected outcome (the API returns 404), so Warning rather than Error.
            _logger.LogWarning("Product {ProductId} was not found", request.Id);
            return null;
        }

        return new ProductDto(product.Id, product.Name, product.Price);
    }
}

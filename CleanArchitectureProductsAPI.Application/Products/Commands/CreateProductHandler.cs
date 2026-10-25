using CleanArchitectureProductsAPI.Application.Abstractions;
using CleanArchitectureProductsAPI.Application.DTOs;
using CleanArchitectureProductsAPI.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureProductsAPI.Application.Products.Commands;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _repository;
    private readonly ILogger<CreateProductHandler> _logger;

    public CreateProductHandler(IProductRepository repository, ILogger<CreateProductHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ProductDto> Handle(
        CreateProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating product with name {ProductName} and price {Price}",
            request.Name, request.Price);

        // Validation already ran in the pipeline, so Name is not null here.
        var product = new Product(request.Name!, request.Price);

        await _repository.AddAsync(product, cancellationToken);

        // After saving, EF Core has filled in product.Id.
        // Unexpected failures are not caught here: they propagate to GlobalExceptionHandler
        // in the API layer, which logs them once.
        _logger.LogInformation("Product {ProductId} was created successfully", product.Id);

        return new ProductDto(product.Id, product.Name, product.Price);
    }
}

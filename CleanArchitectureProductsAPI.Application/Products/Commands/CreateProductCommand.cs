using CleanArchitectureProductsAPI.Application.DTOs;
using MediatR;

namespace CleanArchitectureProductsAPI.Application.Products.Commands;

public record CreateProductCommand(string? Name, decimal Price) : IRequest<ProductDto>;

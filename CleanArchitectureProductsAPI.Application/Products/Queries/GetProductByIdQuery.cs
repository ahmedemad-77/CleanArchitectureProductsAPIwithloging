using CleanArchitectureProductsAPI.Application.DTOs;
using MediatR;

namespace CleanArchitectureProductsAPI.Application.Products.Queries;

// Returns null when the product does not exist.
public record GetProductByIdQuery(int Id) : IRequest<ProductDto?>;

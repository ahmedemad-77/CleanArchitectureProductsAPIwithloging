using CleanArchitectureProductsAPI.Application.DTOs;
using MediatR;

namespace CleanArchitectureProductsAPI.Application.Products.Queries;

public record GetProductsQuery() : IRequest<List<ProductDto>>;

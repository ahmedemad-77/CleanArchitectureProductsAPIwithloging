namespace CleanArchitectureProductsAPI.Application.DTOs;

// Name is nullable so that a missing "name" reaches OUR validator
// (one source of validation rules) instead of ASP.NET's automatic check.
public record CreateProductDto(string? Name, decimal Price);

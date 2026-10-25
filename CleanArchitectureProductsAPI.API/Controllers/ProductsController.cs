using CleanArchitectureProductsAPI.Application.DTOs;
using CleanArchitectureProductsAPI.Application.Products.Commands;
using CleanArchitectureProductsAPI.Application.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureProductsAPI.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(ISender sender, ILogger<ProductsController> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        _logger.LogDebug("API request: get all products");
        var products = await _sender.Send(new GetProductsQuery(), cancellationToken);
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        _logger.LogDebug("API request: get product {ProductId}", id);
        var product = await _sender.Send(new GetProductByIdQuery(id), cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateProductDto dto, CancellationToken cancellationToken)
    {
        _logger.LogDebug("API request: create product");
        var command = new CreateProductCommand(dto.Name, dto.Price);
        var created = await _sender.Send(command, cancellationToken);

        // 201 Created + Location header: /api/products/{id}
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}

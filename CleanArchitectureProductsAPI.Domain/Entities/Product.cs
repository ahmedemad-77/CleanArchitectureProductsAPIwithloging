namespace CleanArchitectureProductsAPI.Domain.Entities;

public class Product
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }

    // Required by EF Core when it materializes entities from the database.
    private Product() { }

    public Product(string name, decimal price)
    {
        // Last line of defense: the entity never allows an invalid state.
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required.", nameof(name));
        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero.", nameof(price));

        Name = name.Trim();
        Price = price;
    }
}

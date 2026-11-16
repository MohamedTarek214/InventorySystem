namespace ProductInventory.DAL.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public ProductCategory Category { get; set; }
    public bool IsActive { get; set; } = true;

    // Filled by the database (default value) when the row is inserted.
    public DateTime CreatedAt { get; set; }

    public DateTime? ExpiryDate { get; set; }
}

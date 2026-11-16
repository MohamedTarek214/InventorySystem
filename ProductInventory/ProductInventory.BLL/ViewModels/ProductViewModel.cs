using System.ComponentModel.DataAnnotations;
using ProductInventory.BLL.Common;
using ProductInventory.DAL.Entities;

namespace ProductInventory.BLL.ViewModels;

/// <summary>Read-only representation of a product (list, details, delete confirmation).</summary>
public class ProductViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    [Display(Name = "SKU")]
    public string SKU { get; set; } = string.Empty;

    public string? Description { get; set; }
    public decimal Price { get; set; }

    [Display(Name = "Stock quantity")]
    public int StockQuantity { get; set; }

    public ProductCategory Category { get; set; }
    public bool IsActive { get; set; }

    [Display(Name = "Created at")]
    public DateTime CreatedAt { get; set; }

    [Display(Name = "Expiry date")]
    public DateTime? ExpiryDate { get; set; }

    public bool IsLowStock => StockQuantity < ProductRules.LowStockThreshold;
}

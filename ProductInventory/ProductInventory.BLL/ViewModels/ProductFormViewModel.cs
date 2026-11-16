using System.ComponentModel.DataAnnotations;
using ProductInventory.BLL.Validation;
using ProductInventory.DAL.Entities;

namespace ProductInventory.BLL.ViewModels;

/// <summary>Used by the Create and Edit forms.</summary>
public class ProductFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "SKU is required.")]
    [StringLength(20, ErrorMessage = "SKU cannot exceed 20 characters.")]
    [Display(Name = "SKU")]
    public string SKU { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? Description { get; set; }

    [Range(0.01, 1_000_000_000, ErrorMessage = "Price must be greater than 0.")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")]
    [Display(Name = "Stock quantity")]
    public int StockQuantity { get; set; }

    // Nullable so the form can start with "-- Select category --" instead of silently picking Electronics.
    [Required(ErrorMessage = "Please select a category.")]
    public ProductCategory? Category { get; set; }

    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [NotInThePast]
    [Display(Name = "Expiry date")]
    public DateTime? ExpiryDate { get; set; }
}

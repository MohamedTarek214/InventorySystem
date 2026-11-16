using System.ComponentModel.DataAnnotations;

namespace ProductInventory.BLL.ViewModels;

public class RestockViewModel
{
    public int Id { get; set; }

    // Display-only values, filled from the database.
    public string ProductName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int CurrentStock { get; set; }

    [Required(ErrorMessage = "Quantity is required.")]
    [Range(1, 100_000, ErrorMessage = "Quantity must be greater than 0 (maximum 100000).")]
    [Display(Name = "Quantity to add")]
    public int? Quantity { get; set; }
}

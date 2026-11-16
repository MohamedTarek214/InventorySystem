using ProductInventory.BLL.Common;
using ProductInventory.DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductInventory.BLL.ViewModels
{
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

}

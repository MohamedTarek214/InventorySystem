using ProductInventory.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductInventory.BLL.ViewModels;


namespace ProductInventory.BLL.Mappings
{
    public static class ProductMappings
    {
        public static ProductViewModel ToViewModel(this Product product) => new()
        {
            Id = product.Id,
            Name = product.Name,
            SKU = product.SKU,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            Category = product.Category,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            ExpiryDate = product.ExpiryDate
        };

        public static ProductFormViewModel ToFormViewModel(this Product product) => new()
        {
            Id = product.Id,
            Name = product.Name,
            SKU = product.SKU,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            Category = product.Category,
            ExpiryDate = product.ExpiryDate
        };

        public static RestockViewModel ToRestockViewModel(this Product product) => new()
        {
            Id = product.Id,
            ProductName = product.Name,
            SKU = product.SKU,
            CurrentStock = product.StockQuantity
        };
    }

}

using ProductInventory.BLL.Common;
using ProductInventory.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductInventory.BLL.ViewModels;


namespace ProductInventory.BLL.Services
{
    public interface IProductService
    {
        Task<ProductListViewModel> GetProductsAsync(string? search, ProductCategory? category, int page);
        Task<IReadOnlyList<ProductViewModel>> GetLowStockAsync();
        Task<ProductViewModel?> GetByIdAsync(int id);
        Task<ProductFormViewModel?> GetForEditAsync(int id);
        Task<RestockViewModel?> GetForRestockAsync(int id);

        Task<ServiceResult> CreateAsync(ProductFormViewModel model);
        Task<ServiceResult> UpdateAsync(int id, ProductFormViewModel model);
        Task<ServiceResult> DeleteAsync(int id);
        Task<ServiceResult> RestockAsync(int id, int quantity);
    }

}

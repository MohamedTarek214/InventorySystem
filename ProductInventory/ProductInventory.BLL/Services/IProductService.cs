using ProductInventory.BLL.Common;
using ProductInventory.BLL.ViewModels;
using ProductInventory.DAL.Entities;

namespace ProductInventory.BLL.Services;

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

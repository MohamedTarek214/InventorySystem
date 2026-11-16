using ProductInventory.DAL.Entities;

namespace ProductInventory.DAL.Repositories;

public interface IProductRepository
{
    /// <summary>Active products only, filtered and paged.</summary>
    Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedAsync(
        string? search, ProductCategory? category, int page, int pageSize);

    /// <summary>Active products whose stock is below the given threshold.</summary>
    Task<IReadOnlyList<Product>> GetLowStockAsync(int threshold);

    /// <summary>Returns the product (tracked) or null. Does not filter on IsActive.</summary>
    Task<Product?> GetByIdAsync(int id);

    Task<bool> SkuExistsAsync(string sku, int? excludeProductId = null);

    Task AddAsync(Product product);

    Task UpdateAsync(Product product);
}

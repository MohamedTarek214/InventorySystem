using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProductInventory.DAL.Data;
using ProductInventory.DAL.Entities;
using ProductInventory.DAL.Exceptions;

namespace ProductInventory.DAL.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedAsync(
        string? search, ProductCategory? category, int page, int pageSize)
    {
        // Soft-deleted (inactive) products are hidden from the list.
        var query = _context.Products
            .AsNoTracking()
            .Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(p => p.Name.Contains(term) || p.SKU.Contains(term));
        }

        if (category.HasValue)
        {
            var selected = category.Value;
            query = query.Where(p => p.Category == selected);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(p => p.Name)
            .ThenBy(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<Product>> GetLowStockAsync(int threshold)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.IsActive && p.StockQuantity < threshold)
            .OrderBy(p => p.StockQuantity)
            .ThenBy(p => p.Name)
            .ToListAsync();
    }

    public Task<Product?> GetByIdAsync(int id)
    {
        return _context.Products.FirstOrDefaultAsync(p => p.Id == id);
    }

    public Task<bool> SkuExistsAsync(string sku, int? excludeProductId = null)
    {
        return _context.Products.AnyAsync(p =>
            p.SKU == sku && (excludeProductId == null || p.Id != excludeProductId));
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await SaveAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await SaveAsync();
    }

    private async Task SaveAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            // 2601 = duplicate key in a unique index, 2627 = unique constraint violation.
            throw new DuplicateSkuException(ex);
        }
    }

    private static bool IsUniqueViolation(DbUpdateException ex)
        => ex.InnerException is SqlException { Number: 2601 or 2627 };
}

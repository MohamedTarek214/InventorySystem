using ProductInventory.BLL.Common;
using ProductInventory.BLL.Mappings;
using ProductInventory.BLL.ViewModels;
using ProductInventory.DAL.Entities;
using ProductInventory.DAL.Exceptions;
using ProductInventory.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProductInventory.BLL.Services
{
    public class ProductService : IProductService
    {
        private const string SkuTakenMessage = "A product with this SKU already exists. Please use a different SKU.";
        private const string GroceryExpiryMessage = "Expiry date is required for Grocery products.";

        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        // ---------- Read ----------

        public async Task<ProductListViewModel> GetProductsAsync(string? search, ProductCategory? category, int page)
        {
            const int pageSize = ProductRules.PageSize;
            page = Math.Max(1, page);

            var (items, totalCount) = await _repository.GetPagedAsync(search, category, page, pageSize);

            // The requested page is past the end (e.g. after a filter): go back to the last page.
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            if (totalPages > 0 && page > totalPages)
            {
                page = totalPages;
                (items, totalCount) = await _repository.GetPagedAsync(search, category, page, pageSize);
            }

            return new ProductListViewModel
            {
                Products = items.Select(p => p.ToViewModel()).ToList(),
                Search = search,
                Category = category,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<IReadOnlyList<ProductViewModel>> GetLowStockAsync()
        {
            var products = await _repository.GetLowStockAsync(ProductRules.LowStockThreshold);
            return products.Select(p => p.ToViewModel()).ToList();
        }

        public async Task<ProductViewModel?> GetByIdAsync(int id)
        {
            var product = await FindActiveAsync(id);
            return product?.ToViewModel();
        }

        public async Task<ProductFormViewModel?> GetForEditAsync(int id)
        {
            var product = await FindActiveAsync(id);
            return product?.ToFormViewModel();
        }

        public async Task<RestockViewModel?> GetForRestockAsync(int id)
        {
            var product = await FindActiveAsync(id);
            return product?.ToRestockViewModel();
        }

        // ---------- Create ----------

        public async Task<ServiceResult> CreateAsync(ProductFormViewModel model)
        {
            var sku = NormalizeSku(model.SKU);

            var violation = await CheckBusinessRulesAsync(model, sku, excludeProductId: null);
            if (violation is not null)
            {
                return violation;
            }

            var product = new Product
            {
                Name = model.Name.Trim(),
                SKU = sku,
                Description = NormalizeDescription(model.Description),
                Price = model.Price,
                StockQuantity = model.StockQuantity,
                Category = model.Category!.Value,
                ExpiryDate = model.ExpiryDate?.Date
            };

            try
            {
                await _repository.AddAsync(product);
            }
            catch (DuplicateSkuException)
            {
                // Another request saved the same SKU between our check and the insert.
                return ServiceResult.Invalid(nameof(ProductFormViewModel.SKU), SkuTakenMessage);
            }

            return ServiceResult.Ok();
        }

        // ---------- Update ----------

        public async Task<ServiceResult> UpdateAsync(int id, ProductFormViewModel model)
        {
            var product = await FindActiveAsync(id);
            if (product is null)
            {
                return ServiceResult.NotFound();
            }

            var sku = NormalizeSku(model.SKU);

            var violation = await CheckBusinessRulesAsync(model, sku, excludeProductId: id);
            if (violation is not null)
            {
                return violation;
            }

            product.Name = model.Name.Trim();
            product.SKU = sku;
            product.Description = NormalizeDescription(model.Description);
            product.Price = model.Price;
            product.StockQuantity = model.StockQuantity;
            product.Category = model.Category!.Value;
            product.ExpiryDate = model.ExpiryDate?.Date;

            try
            {
                await _repository.UpdateAsync(product);
            }
            catch (DuplicateSkuException)
            {
                return ServiceResult.Invalid(nameof(ProductFormViewModel.SKU), SkuTakenMessage);
            }

            return ServiceResult.Ok();
        }

        // ---------- Delete (soft delete) ----------

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var product = await FindActiveAsync(id);
            if (product is null)
            {
                return ServiceResult.NotFound();
            }

            // Soft delete: keep the row, just hide it.
            product.IsActive = false;
            await _repository.UpdateAsync(product);

            return ServiceResult.Ok();
        }

        // ---------- Restock ----------

        public async Task<ServiceResult> RestockAsync(int id, int quantity)
        {
            if (quantity <= 0)
            {
                return ServiceResult.Invalid(nameof(RestockViewModel.Quantity), "Quantity must be greater than 0.");
            }

            var product = await FindActiveAsync(id);
            if (product is null)
            {
                return ServiceResult.NotFound();
            }

            if ((long)product.StockQuantity + quantity > int.MaxValue)
            {
                return ServiceResult.Invalid(nameof(RestockViewModel.Quantity), "The resulting stock quantity is too large.");
            }

            product.StockQuantity += quantity;
            await _repository.UpdateAsync(product);

            return ServiceResult.Ok();
        }

        // ---------- Helpers ----------

        /// <summary>Inactive (soft-deleted) products are treated as "not found".</summary>
        private async Task<Product?> FindActiveAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            return product is { IsActive: true } ? product : null;
        }

        /// <summary>Business rules that need more than a single field: Grocery expiry + unique SKU.</summary>
        private async Task<ServiceResult?> CheckBusinessRulesAsync(ProductFormViewModel model, string sku, int? excludeProductId)
        {
            if (model.Category == ProductCategory.Grocery && model.ExpiryDate is null)
            {
                return ServiceResult.Invalid(nameof(ProductFormViewModel.ExpiryDate), GroceryExpiryMessage);
            }

            if (await _repository.SkuExistsAsync(sku, excludeProductId))
            {
                return ServiceResult.Invalid(nameof(ProductFormViewModel.SKU), SkuTakenMessage);
            }

            return null;
        }

        private static string NormalizeSku(string sku) => sku.Trim().ToUpperInvariant();

        private static string? NormalizeDescription(string? description)
            => string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

}

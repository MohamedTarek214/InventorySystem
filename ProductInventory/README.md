# Product Inventory System

A small store inventory app built with **ASP.NET Core MVC (.NET 8)**, **EF Core (Code First, SQL Server)**,
the **Repository pattern**, **Dependency Injection** and an **N-tier** layout.

## Solution structure

| Project | Layer | Contains |
|---|---|---|
| `ProductInventory.DAL` | Data Access | `Product` entity, `ProductCategory` enum, `AppDbContext`, `ProductConfiguration` (Fluent API), `IProductRepository` / `ProductRepository`, migrations |
| `ProductInventory.BLL` | Business Logic | `IProductService` / `ProductService`, ViewModels (with validation attributes), `NotInThePastAttribute`, `ServiceResult`, mappings |
| `ProductInventory.PL` | Presentation | MVC controllers, Razor views, `Program.cs`, client-side script |

Dependencies go one way: `PL -> BLL -> DAL`. Controllers and views talk to `IProductService` only.

## Getting started

1. Edit the connection string in `ProductInventory.PL/appsettings.json` (use the server name shown in SSMS).
2. Apply the migrations (Package Manager Console, **Default project = ProductInventory.DAL**, **Startup project = ProductInventory.PL**):
   ```powershell
   Add-Migration InitialCreate
   Update-Database
   ```
   Or from a terminal in the solution folder:
   ```bash
   dotnet ef migrations add InitialCreate --project ProductInventory.DAL --startup-project ProductInventory.PL
   dotnet ef database update --project ProductInventory.DAL --startup-project ProductInventory.PL
   ```
3. Run `ProductInventory.PL` (F5).

## How the repository and dependency injection are applied

* `IProductRepository` / `ProductRepository` (DAL) is a **specific, non-generic** repository. It is the only class
  that queries `AppDbContext`: paged search, low-stock query, get by id, SKU exists check, add and update.
* `ProductService` (BLL) receives `IProductRepository` in its constructor and holds all business logic.
* `ProductsController` (PL) receives `IProductService` in its constructor.
* Everything is registered in `Program.cs`:
  ```csharp
  builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
  builder.Services.AddScoped<IProductRepository, ProductRepository>();
  builder.Services.AddScoped<IProductService, ProductService>();
  ```
  `Program.cs` is the composition root, so it is the only place in the PL that mentions the DbContext and the repository.
  Scoped lifetime = one DbContext, one repository and one service per HTTP request.

## How CRUD works

| Operation | Action | What happens |
|---|---|---|
| Read (list) | `GET /Products` | `ProductService.GetProductsAsync` -> repository returns **active** products, filtered by name/SKU/category and paged (10 per page) |
| Read (one) | `GET /Products/Details/{id}` | `GetByIdAsync`; unknown or inactive id -> `404 NotFound` |
| Create | `GET/POST /Products/Create` | ModelState check -> `ProductService.CreateAsync` (business rules) -> `repository.AddAsync` -> redirect to list |
| Update | `GET/POST /Products/Edit/{id}` | Same flow with `UpdateAsync`; unknown id -> `404` |
| Delete | `GET/POST /Products/Delete/{id}` | **Soft delete**: `IsActive = false`; the row stays in the table and disappears from the list |
| Restock | `GET/POST /Products/Restock/{id}` | Adds a quantity (> 0) to the current stock |
| Low stock | `GET /Products/LowStock` | Active products with `StockQuantity < 5` |

Search/filter/paging/low stock/restock/soft delete/N-tier are the bonus features and are all implemented.

## Where each validation rule is enforced

| Rule | Client side | Server side | Database |
|---|---|---|---|
| Name required, max 100 | unobtrusive (from attributes) | `[Required]`, `[StringLength(100)]` on `ProductFormViewModel` | `nvarchar(100) NOT NULL` |
| SKU required, max 20 | unobtrusive | `[Required]`, `[StringLength(20)]` | `nvarchar(20) NOT NULL` |
| SKU unique | - | `ProductService.CheckBusinessRulesAsync` (`SkuExistsAsync`) shows a clear error on the SKU field; a race condition is caught as `DuplicateSkuException` | unique index `IX_Products_SKU` |
| Description max 500 | unobtrusive | `[StringLength(500)]` | `nvarchar(500)` |
| Price > 0 | unobtrusive | `[Range(0.01, ...)]` | `decimal(18,2)` + check constraint `CK_Products_Price` |
| StockQuantity >= 0 | unobtrusive | `[Range(0, int.MaxValue)]` | check constraint `CK_Products_StockQuantity` |
| ExpiryDate not in the past | `product-form.js` (`notinthepast`) | `NotInThePastAttribute` (BLL) | - |
| **Grocery requires ExpiryDate** | `product-form.js` (`requiredforgrocery`, re-checked when the category changes) | `ProductService.CheckBusinessRulesAsync` (`Category == Grocery && ExpiryDate is null`) | check constraint `CK_Products_GroceryExpiry` |
| Id must exist | - | service returns `ResultStatus.NotFound` -> controller returns `NotFound()` | - |
| Restock quantity > 0 | unobtrusive | `[Range(1, 100000)]` + check in `RestockAsync` | - |

### Fluent API (`ProductConfiguration`)
Max lengths, `Price` precision (18,2), unique index on SKU, `Category` stored as a string (`HasConversion<string>()`),
`IsActive` default `true` and `CreatedAt` default `GETUTCDATE()` (both **database-side**).

## Notes
* Soft-deleted products keep their SKU, so the SKU cannot be reused (the unique index still applies).
* The UI uses Bootstrap and jQuery validation from a CDN (internet connection required in the browser).

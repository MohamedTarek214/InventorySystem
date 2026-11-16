using Microsoft.AspNetCore.Mvc;
using ProductInventory.BLL.Common;
using ProductInventory.BLL.Services;
using ProductInventory.BLL.ViewModels;
using ProductInventory.DAL.Entities;

namespace ProductInventory.PL.Controllers;

public class ProductsController : Controller
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // GET: /Products?search=abc&category=Grocery&page=2
    public async Task<IActionResult> Index(string? search, ProductCategory? category, int page = 1)
    {
        var model = await _productService.GetProductsAsync(search, category, page);
        return View(model);
    }

    // GET: /Products/LowStock
    public async Task<IActionResult> LowStock()
    {
        var model = await _productService.GetLowStockAsync();
        return View(model);
    }

    // GET: /Products/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product is null)
        {
            return NotFound();
        }

        return View(product);
    }

    // ---------- Create ----------

    public IActionResult Create()
    {
        return View(new ProductFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _productService.CreateAsync(model);
        if (!result.Succeeded)
        {
            AddErrors(result);
            return View(model);
        }

        TempData["Success"] = "Product created successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ---------- Edit ----------

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _productService.GetForEditAsync(id);
        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _productService.UpdateAsync(id, model);
        if (result.Status == ResultStatus.NotFound)
        {
            return NotFound();
        }

        if (!result.Succeeded)
        {
            AddErrors(result);
            return View(model);
        }

        TempData["Success"] = "Product updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ---------- Delete (soft delete) ----------

    public async Task<IActionResult> Delete(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product is null)
        {
            return NotFound();
        }

        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var result = await _productService.DeleteAsync(id);
        if (result.Status == ResultStatus.NotFound)
        {
            return NotFound();
        }

        TempData["Success"] = "Product deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ---------- Restock ----------

    public async Task<IActionResult> Restock(int id)
    {
        var model = await _productService.GetForRestockAsync(id);
        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restock(int id, RestockViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            var result = await _productService.RestockAsync(id, model.Quantity!.Value);
            if (result.Status == ResultStatus.NotFound)
            {
                return NotFound();
            }

            if (result.Succeeded)
            {
                TempData["Success"] = $"Stock updated: {model.Quantity} item(s) added.";
                return RedirectToAction(nameof(Index));
            }

            AddErrors(result);
        }

        // Re-load the display values (name, current stock) and keep what the user typed.
        var view = await _productService.GetForRestockAsync(id);
        if (view is null)
        {
            return NotFound();
        }

        view.Quantity = model.Quantity;
        return View(view);
    }

    private void AddErrors(ServiceResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(error.Field, error.Message);
        }
    }
}

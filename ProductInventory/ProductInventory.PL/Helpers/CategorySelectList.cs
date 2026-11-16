using Microsoft.AspNetCore.Mvc.Rendering;
using ProductInventory.DAL.Entities;

namespace ProductInventory.PL.Helpers;

public static class CategorySelectList
{
    /// <summary>Category names as both text and value (e.g. "Grocery"), so the client script can read them.</summary>
    public static IEnumerable<SelectListItem> Items =>
        Enum.GetNames<ProductCategory>().Select(name => new SelectListItem(name, name)).ToList();
}

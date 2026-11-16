namespace ProductInventory.BLL.Common;

public static class ProductRules
{
    /// <summary>Products with stock below this number appear in the low-stock list.</summary>
    public const int LowStockThreshold = 5;

    public const int PageSize = 10;
}

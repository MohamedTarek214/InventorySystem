using ProductInventory.DAL.Entities;

namespace ProductInventory.BLL.ViewModels;

/// <summary>One page of products plus the current search / filter / paging state.</summary>
public class ProductListViewModel
{
    public IReadOnlyList<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();

    public string? Search { get; set; }
    public ProductCategory? Category { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
}

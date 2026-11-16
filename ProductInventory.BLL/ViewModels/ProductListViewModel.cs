using ProductInventory.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductInventory.BLL.ViewModels
{
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

}

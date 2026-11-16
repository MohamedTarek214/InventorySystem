using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductInventory.DAL.Exceptions
{
    public class DuplicateSkuException : Exception
    {
        public DuplicateSkuException(Exception innerException)
            : base("A product with the same SKU already exists.", innerException)
        {
        }
    }
}

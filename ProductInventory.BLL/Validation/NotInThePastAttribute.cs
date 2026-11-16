using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductInventory.BLL.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class NotInThePastAttribute : ValidationAttribute
    {
        public NotInThePastAttribute() : base("Expiry date cannot be in the past.")
        {
        }

        public override bool IsValid(object? value)
        {
            // Empty values are allowed here; "required" rules are checked elsewhere.
            if (value is null)
            {
                return true;
            }

            return value is DateTime date && date.Date >= DateTime.Today;
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace ProductInventory.BLL.Validation;

/// <summary>
/// Server-side rule: a date (when provided) must be today or later.
/// The same rule is mirrored on the client in wwwroot/js/product-form.js.
/// </summary>
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

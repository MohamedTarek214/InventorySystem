namespace ProductInventory.DAL.Exceptions;

/// <summary>
/// Thrown when the unique index on SKU is violated (e.g. two users save the same SKU at the same time).
/// </summary>
public class DuplicateSkuException : Exception
{
    public DuplicateSkuException(Exception innerException)
        : base("A product with the same SKU already exists.", innerException)
    {
    }
}

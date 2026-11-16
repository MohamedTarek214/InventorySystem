using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductInventory.DAL.Entities;

namespace ProductInventory.DAL.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products", t =>
        {
            // Last line of defence: the same business rules enforced in the database.
            t.HasCheckConstraint("CK_Products_Price", "[Price] > 0");
            t.HasCheckConstraint("CK_Products_StockQuantity", "[StockQuantity] >= 0");
            t.HasCheckConstraint("CK_Products_GroceryExpiry", "[Category] <> 'Grocery' OR [ExpiryDate] IS NOT NULL");
        });

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.SKU)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.Price)
            .HasPrecision(18, 2);

        // Stored as text ("Grocery") instead of an int.
        builder.Property(p => p.Category)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // Database-side default = true. The sentinel tells EF Core that "true" means
        // "let the database decide", so an explicit false is still written to the row.
        builder.Property(p => p.IsActive)
            .HasDefaultValue(true)
            .HasSentinel(true);

        // Database-side default = current date/time (UTC).
        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(p => p.SKU)
            .IsUnique()
            .HasDatabaseName("IX_Products_SKU");
    }
}

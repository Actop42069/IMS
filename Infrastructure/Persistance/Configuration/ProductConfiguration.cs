using Domain.Entities;
using Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configuration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure (EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(e => e.ProductId);
            builder.Property(a => a.ProductId)
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property (a => a.Description)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(a => a.SKU)
                .HasColumnType("NVARCHAR")
                .HasMaxLength(400);

            builder.Property(a => a.Price)
                .HasPrecision(10, 2)
                .IsRequired();

            builder.Property(a => a.UnitCost)
                .HasPrecision (10, 2)
                .IsRequired();

            builder.Property(a => a.Quantity)
                .IsRequired();

            builder.Property(a => a.ImageUrl)
                .HasMaxLength (2000)
                .IsRequired(false);

            builder.HasOne(a => a.Category)  // 1:M
                .WithMany(a => a.Product)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.Vendor) // 1:M
                .WithMany(a => a.Product)
                .HasForeignKey (a => a.VendorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configuration
{
    public class SaleProductConfiguration : IEntityTypeConfiguration<SaleProduct>
    {
        public void Configure(EntityTypeBuilder<SaleProduct> builder)
        {
            builder.HasKey(sp => new { sp.SalesId, sp.ProductId });

            builder.Property(sp => sp.Quantity)
                .IsRequired();

            builder.Property(sp => sp.UnitPrice)
                .IsRequired()
                .HasPrecision(10,2);

            builder.HasOne(sp => sp.Sales)
                .WithMany(s => s.SaleProducts)
                .HasForeignKey(sp => sp.SalesId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(sp => sp.Product)
                .WithMany(p => p.SaleProducts)
                .HasForeignKey(sp => sp.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

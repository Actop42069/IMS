using Domain.Entities;
using Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configuration
{
    public class SalesConfiguration : IEntityTypeConfiguration<Sales>
    {
        public void Configure (EntityTypeBuilder<Sales> builder)
        {
            builder.HasKey(a => a.SalesId);
            builder.Property(a => a.SalesId)
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(a => a.TranscationId)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(a => a.Quantity)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasOne(s => s.Customer)
                .WithMany(c => c.Sales)
                .HasForeignKey(s => s.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(s => s.PaymentDate)
                .HasDatabaseName("IX_Sales_PaymentDate");

            builder.HasIndex(s => s.TranscationId)
                .IsUnique();
        }
    }
}

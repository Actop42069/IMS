using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration
{
    public class VendorContactConfiguration : IEntityTypeConfiguration<VendorContact>
    {
        public void Configure(EntityTypeBuilder<VendorContact> builder)
        {
            builder.HasKey(a => a.VendorContactId);

            builder.Property(a => a.VendorContactId)
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Position)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(a => a.Department)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(a => a.Email)
                .IsRequired()
                .HasMaxLength(200);
            builder.HasIndex(a => a.Email)
                .IsUnique();

            builder.Property(a => a.PhoneNumber)
                .IsRequired()
                .HasMaxLength(14);
        }
    }
}
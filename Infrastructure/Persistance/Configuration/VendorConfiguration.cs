using Domain.Entities;
using Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration
{
    public class VendorConfiguration : IEntityTypeConfiguration<Vendor>
    {
        public void Configure (EntityTypeBuilder<Vendor> builder)
        {
            builder.HasKey(e => e.VendorId);

            builder.Property(e => e.VendorId)
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(a => a.VendorName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(a => a.Email)
                .HasMaxLength(100)
                .IsRequired();
            builder.HasIndex(e => e.Email)
                .IsUnique();

            builder.Property(a => a.PhoneNumber)
                .HasMaxLength(13)
                .IsRequired();
            builder.HasIndex(a => a.PhoneNumber)
                .IsUnique();
        }
    }
}
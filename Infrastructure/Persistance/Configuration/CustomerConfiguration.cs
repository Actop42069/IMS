using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configuration
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(a => a.CustomerId);
            builder.Property(p => p.CustomerId)
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(p => p.FirstName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(p => p.LastName)
               .HasMaxLength(50)
               .IsRequired();

            builder.Property(p => p.Email)
               .HasMaxLength(200)
               .IsRequired();
            builder.HasIndex(h => h.Email)
                .IsUnique();

            builder.Property(p => p.PhoneNumber)
               .HasMaxLength(13)
               .IsRequired();
            builder.HasIndex(h => h.PhoneNumber)
                .IsUnique();

            builder.Property(a => a.ImageUrl)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.HasMany(a => a.CustomerDocument)
                .WithOne(a => a.Customer)
                .HasForeignKey(h => h.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

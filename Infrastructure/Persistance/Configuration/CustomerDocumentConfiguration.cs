using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configuration
{
    public class CustomerDocumentConfiguration : IEntityTypeConfiguration<CustomerDocument>
    {
        public void Configure (EntityTypeBuilder<CustomerDocument> builder)
        {
            builder.HasKey(e => e.CustomerDocumentId);
            builder.Property(e => e.CustomerDocumentId)
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.HasOne(h => h.Customer)
               .WithMany(a => a.CustomerDocument)
               .HasForeignKey(h => h.CustomerId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.Property(p => p.DocumentUrl)
                .HasMaxLength(300)
                .IsRequired(true);
        }
    }
}

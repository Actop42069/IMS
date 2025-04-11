using Domain.Entities;
using Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Persistance.Configuration
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(a => a.CategoryId);

            builder.Property(a => a.CategoryId)
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(a => a.CategoryName)
                .HasMaxLength(300)
                .IsRequired();

            builder.Property(a => a.Description)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.HasMany(c => c.Product)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

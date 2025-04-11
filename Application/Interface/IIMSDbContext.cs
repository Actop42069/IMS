using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Domain.Entities;

namespace Application.Interface
{
    public interface IIMSDbContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        DatabaseFacade Database { get; }
        public DbSet<ErrorLog> ErrorLog { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<CustomerDocument> CustomerDocument { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<SaleProduct> SaleProduct { get; set; }
        public DbSet<Sales> Sales { get; set; }
        public DbSet<VendorContact> VendorContact { get; set; }
        public DbSet<Vendor> Vendor { get; set; }
    }
}

using Domain.Enumeration;

namespace Domain.Entities
{
    public class Product : BaseEntity
    {
        public int ProductId { get; set; }
        public int VendorId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; }
        public ProductStatus ProductStatus { get; set; }
        public ProductType ProductType { get; set; }
        public decimal Price { get; set; }
        public decimal UnitCost { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }

        public virtual Vendor Vendor { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<Sales> Sales { get; set; }

        public virtual ICollection<SaleProduct> SaleProducts { get; set; }
    }
}

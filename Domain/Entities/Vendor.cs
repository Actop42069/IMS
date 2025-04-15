using Domain.Enumeration;

namespace Domain.Entities
{
    public class Vendor : BaseEntity
    {
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public VendorType VendorType { get; set; }
        public VendorStatus VendorStatus { get; set; }

        public virtual ICollection<Product> Product { get; set; }
    }
}

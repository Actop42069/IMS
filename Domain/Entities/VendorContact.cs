namespace Domain.Entities
{
    public class VendorContact
    {
        public int VendorContactId { get; set; }
        public int VendorId { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public virtual Vendor Vendor { get; set; }
    }
}

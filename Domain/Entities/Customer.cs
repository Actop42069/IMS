using Domain.Enumeration;

namespace Domain.Entities
{
    public class Customer : BaseEntity
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public CustomerType CustomerType { get; set; }
        public string ImageUrl { get; set; }

        public virtual ICollection<Sales> Sales { get; set; }
        public virtual ICollection<CustomerDocument> CustomerDocument { get; set; }
    }
}

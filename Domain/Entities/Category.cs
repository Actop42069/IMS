using Domain.Enumeration;

namespace Domain.Entities
{
    public class Category : BaseEntity
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }

        public CategoryStatus Status { get; set; }

        public virtual ICollection<Product> Product { get; set; }
    }
}

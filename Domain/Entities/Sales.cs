using Domain.Enumeration;

namespace Domain.Entities
{
    public class Sales : BaseEntity
    {
        public int SalesId { get; set; }
        public int ProductId { get; set; }
        public DateTimeOffset PaymentDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string TranscationId { get; set; }
        public OrderType PaymentStatus { get; set; }
        public int Quantity { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public CustomerType CustomerType { get; set; }

        public virtual ICollection<SaleProduct> SaleProducts { get; set; }
    }
}

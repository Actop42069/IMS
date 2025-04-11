using Domain.Enumeration;

namespace Domain.Entities
{
    public class Sales : BaseEntity
    {
        public int SalesId { get; set; }
        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public DateTimeOffset PaymentDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string TranscationId { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public int Quantity { get; set; }

        public virtual ICollection<SaleProduct> SaleProducts { get; set; }
        public virtual Customer Customer { get; set; }
    }
}

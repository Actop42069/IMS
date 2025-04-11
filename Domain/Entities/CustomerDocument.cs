using Domain.Enumeration;

namespace Domain.Entities
{
    public class CustomerDocument : BaseEntity
    {
        public long CustomerDocumentId { get; set; }
        public int CustomerId { get; set; }
        public DocumentType Document { get; set; }
        public string DocumentUrl { get; set; }

        public virtual Customer Customer { get; set; }
    }
}

namespace Domain.Entities
{
    public class BaseEntity
    {
        public bool IsActive { get; set; }
        public DateTimeOffset LastUpdatedAt { get; set; } = DateTimeOffset.UtcNow;
        public string LastUpdatedBy { get; set; } = "a";
    }
}

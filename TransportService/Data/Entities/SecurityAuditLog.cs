namespace TransportService.Data.Entities
{
    public class SecurityAuditLog : BaseEntity
    {
        public Guid Id { get; set; }
        public string EventType { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string IpAddress { get; set; } = null!;
        public string UserAgent { get; set; } = null!;
        public bool Success { get; set; }
    }
}

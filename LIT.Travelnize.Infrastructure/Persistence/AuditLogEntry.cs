namespace LIT.Travelnize.Infrastructure.Persistence
{
    public class AuditLogEntry
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CorrelationId { get; internal set; }
        public Guid AggregateId { get; set; }
        public string EntityName { get; set; } = default!;
        public Guid EntityId { get; set; }
        public string Action { get; set; } = default!; // "Added", "Modified", "Deleted"
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public DateTime Timestamp { get; set; }
        public string Details { get; set; } = default!;
        public string RequestInfo { get; set; } = default!;
    }
}
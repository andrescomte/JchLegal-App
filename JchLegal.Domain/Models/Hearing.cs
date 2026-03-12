namespace JchLegal.Domain.Models
{
    public class Hearing
    {
        public Guid Id { get; set; }
        public Guid CaseId { get; set; }
        public int TenantId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public TimeOnly? Time { get; set; }
        public string? Juzgado { get; set; }
        public short StatusId { get; set; }
        public string? Notes { get; set; }
        public short AlertDaysBefore { get; set; }
        public DateTime CreatedAt { get; set; }

        public Case Case { get; set; } = null!;
        public HearingStatus Status { get; set; } = null!;
    }
}

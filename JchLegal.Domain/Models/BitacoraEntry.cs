namespace JchLegal.Domain.Models
{
    public class BitacoraEntry
    {
        public Guid Id { get; set; }
        public Guid CaseId { get; set; }
        public DateOnly Date { get; set; }
        public short EventTypeId { get; set; }
        public string? Description { get; set; }
        public bool VisibleToClient { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public Case Case { get; set; } = null!;
        public BitacoraEventType EventType { get; set; } = null!;
        public ICollection<Attachment> Attachments { get; set; } = [];
    }
}

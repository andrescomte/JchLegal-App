namespace JchLegal.ApplicationApi.Application.DTOs
{
    public class AttachmentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public long? Size { get; set; }
        public string? MimeType { get; set; }
        public DateTime UploadedAt { get; set; }
    }

    public class BitacoraEntryDto
    {
        public Guid Id { get; set; }
        public Guid CaseId { get; set; }
        public DateOnly Date { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool VisibleToClient { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public IEnumerable<AttachmentDto> Attachments { get; set; } = [];
    }
}

namespace JchLegal.Domain.Models
{
    public class Attachment
    {
        public Guid Id { get; set; }
        public Guid BitacoraEntryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public long? Size { get; set; }
        public string? MimeType { get; set; }
        public DateTime UploadedAt { get; set; }

        public BitacoraEntry BitacoraEntry { get; set; } = null!;
    }
}

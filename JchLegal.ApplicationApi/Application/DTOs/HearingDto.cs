namespace JchLegal.ApplicationApi.Application.DTOs
{
    public class HearingDto
    {
        public Guid Id { get; set; }
        public Guid CaseId { get; set; }
        public string? CaseNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public string? Time { get; set; }
        public string? Juzgado { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public short AlertDaysBefore { get; set; }
    }
}

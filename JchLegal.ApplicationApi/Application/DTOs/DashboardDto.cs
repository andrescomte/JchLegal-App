namespace JchLegal.ApplicationApi.Application.DTOs
{
    public class DashboardSummaryDto
    {
        public int ActiveCases { get; set; }
        public int TotalClients { get; set; }
        public int UpcomingHearings { get; set; }
        public decimal PendingFeesAmount { get; set; }
    }
}

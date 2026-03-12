namespace JchLegal.ApplicationApi.Application.DTOs
{
    public class PaymentDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public DateOnly Date { get; set; }
        public string Method { get; set; } = string.Empty;
        public string? Note { get; set; }
        public string? ReceiptUrl { get; set; }
    }

    public class FeeDto
    {
        public Guid Id { get; set; }
        public Guid CaseId { get; set; }
        public string Concepto { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "CLP";
        public string Status { get; set; } = string.Empty;
        public IEnumerable<PaymentDto> Payments { get; set; } = [];
    }
}

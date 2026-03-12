namespace JchLegal.Domain.Models
{
    public class Payment
    {
        public Guid Id { get; set; }
        public Guid FeeId { get; set; }
        public decimal Amount { get; set; }
        public DateOnly Date { get; set; }
        public short MethodId { get; set; }
        public string? Note { get; set; }
        public string? ReceiptUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        public Fee Fee { get; set; } = null!;
        public PaymentMethod Method { get; set; } = null!;
    }
}

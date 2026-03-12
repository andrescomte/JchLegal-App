namespace JchLegal.Domain.Models
{
    public class Fee
    {
        public Guid Id { get; set; }
        public Guid CaseId { get; set; }
        public short ConceptoId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "CLP";
        public DateTime CreatedAt { get; set; }

        public Case Case { get; set; } = null!;
        public FeeConcepto Concepto { get; set; } = null!;
        public ICollection<Payment> Payments { get; set; } = [];
    }
}

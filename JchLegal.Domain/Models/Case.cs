namespace JchLegal.Domain.Models
{
    public class Case
    {
        public Guid Id { get; set; }
        public int TenantId { get; set; }
        public string? Expediente { get; set; }
        public string? Caratulado { get; set; }
        public short MateriaId { get; set; }
        public string? Juzgado { get; set; }
        public short StatusId { get; set; }
        public long? AssignedLawyerId { get; set; }
        public Guid ClientId { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? ClosedDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public CaseMateria Materia { get; set; } = null!;
        public CaseStatus Status { get; set; } = null!;
        public Client Client { get; set; } = null!;
        public ICollection<CasePart> Parts { get; set; } = [];
        public ICollection<BitacoraEntry> BitacoraEntries { get; set; } = [];
        public ICollection<Hearing> Hearings { get; set; } = [];
        public Fee? Fee { get; set; }
    }
}

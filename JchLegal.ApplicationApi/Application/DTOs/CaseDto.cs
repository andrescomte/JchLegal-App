namespace JchLegal.ApplicationApi.Application.DTOs
{
    public class CasePartDataDto
    {
        public string? Type { get; set; }
        public string? Nombre { get; set; }
        public string? Rut { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? RazonSocial { get; set; }
        public string? RepresentanteLegal { get; set; }
    }

    public class CasePartDto
    {
        public Guid Id { get; set; }
        public string Role { get; set; } = string.Empty;
        public CasePartDataDto Data { get; set; } = new();
    }

    public class CaseDto
    {
        public Guid Id { get; set; }
        public string? Expediente { get; set; }
        public string? Caratulado { get; set; }
        public string Materia { get; set; } = string.Empty;
        public string? Juzgado { get; set; }
        public string Status { get; set; } = string.Empty;
        public IEnumerable<CasePartDto> Parts { get; set; } = [];
        public long? AssignedLawyerId { get; set; }
        public Guid ClientId { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? ClosedDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

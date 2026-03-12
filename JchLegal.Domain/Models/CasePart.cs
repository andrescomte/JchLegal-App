namespace JchLegal.Domain.Models
{
    public class CasePart
    {
        public Guid Id { get; set; }
        public Guid CaseId { get; set; }
        public short RoleId { get; set; }
        public short ClientTypeId { get; set; }
        public string? Nombre { get; set; }
        public string? Rut { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? RazonSocial { get; set; }
        public string? RepresentanteLegal { get; set; }

        public Case Case { get; set; } = null!;
        public CasePartRole Role { get; set; } = null!;
        public ClientType ClientType { get; set; } = null!;
    }
}

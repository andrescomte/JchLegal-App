namespace JchLegal.Domain.Models
{
    public class Client
    {
        public Guid Id { get; set; }
        public int TenantId { get; set; }
        public short ClientTypeId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Rut { get; set; }
        public string? RazonSocial { get; set; }
        public string? RutEmpresa { get; set; }
        public string? RepresentanteLegal { get; set; }
        public string? ContactoNombre { get; set; }
        public string? ContactoTelefono { get; set; }
        public string? ContactoEmail { get; set; }
        public long? UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        public ClientType ClientType { get; set; } = null!;
        public ICollection<Case> Cases { get; set; } = [];
    }
}

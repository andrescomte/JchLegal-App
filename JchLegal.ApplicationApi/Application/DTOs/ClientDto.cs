namespace JchLegal.ApplicationApi.Application.DTOs
{
    public class ClientContactDto
    {
        public string? Nombre { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
    }

    public class ClientDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Rut { get; set; }
        public string? RazonSocial { get; set; }
        public string? RutEmpresa { get; set; }
        public string? RepresentanteLegal { get; set; }
        public ClientContactDto Contacto { get; set; } = new();
        public long? UserId { get; set; }
        public IEnumerable<Guid> AssignedCases { get; set; } = [];
        public DateTime CreatedAt { get; set; }
    }
}

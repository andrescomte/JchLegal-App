using JchLegal.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace JchLegal.Infrastructure.Context
{
    public class JchLegalDbContext : DbContext
    {
        public JchLegalDbContext(DbContextOptions<JchLegalDbContext> options) : base(options) { }

        // Lookup tables
        public DbSet<ClientType> ClientTypes => Set<ClientType>();
        public DbSet<CaseMateria> CaseMaterias => Set<CaseMateria>();
        public DbSet<CaseStatus> CaseStatuses => Set<CaseStatus>();
        public DbSet<CasePartRole> CasePartRoles => Set<CasePartRole>();
        public DbSet<BitacoraEventType> BitacoraEventTypes => Set<BitacoraEventType>();
        public DbSet<HearingStatus> HearingStatuses => Set<HearingStatus>();
        public DbSet<FeeConcepto> FeeConceptos => Set<FeeConcepto>();
        public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();

        // Main tables
        public DbSet<Client> Clients => Set<Client>();
        public DbSet<Case> Cases => Set<Case>();
        public DbSet<CasePart> CaseParts => Set<CasePart>();
        public DbSet<BitacoraEntry> BitacoraEntries => Set<BitacoraEntry>();
        public DbSet<Attachment> Attachments => Set<Attachment>();
        public DbSet<Hearing> Hearings => Set<Hearing>();
        public DbSet<Fee> Fees => Set<Fee>();
        public DbSet<Payment> Payments => Set<Payment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Lookup tables ──────────────────────────────────────────────
            modelBuilder.Entity<ClientType>(e =>
            {
                e.ToTable("client_types");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.Code).HasColumnName("code").HasMaxLength(20).IsRequired();
                e.Property(x => x.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
                e.HasIndex(x => x.Code).IsUnique();
            });

            modelBuilder.Entity<CaseMateria>(e =>
            {
                e.ToTable("case_materias");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.Code).HasColumnName("code").HasMaxLength(30).IsRequired();
                e.Property(x => x.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
                e.HasIndex(x => x.Code).IsUnique();
            });

            modelBuilder.Entity<CaseStatus>(e =>
            {
                e.ToTable("case_statuses");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.Code).HasColumnName("code").HasMaxLength(30).IsRequired();
                e.Property(x => x.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
                e.HasIndex(x => x.Code).IsUnique();
            });

            modelBuilder.Entity<CasePartRole>(e =>
            {
                e.ToTable("case_part_roles");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.Code).HasColumnName("code").HasMaxLength(20).IsRequired();
                e.Property(x => x.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
                e.HasIndex(x => x.Code).IsUnique();
            });

            modelBuilder.Entity<BitacoraEventType>(e =>
            {
                e.ToTable("bitacora_event_types");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.Code).HasColumnName("code").HasMaxLength(20).IsRequired();
                e.Property(x => x.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
                e.HasIndex(x => x.Code).IsUnique();
            });

            modelBuilder.Entity<HearingStatus>(e =>
            {
                e.ToTable("hearing_statuses");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.Code).HasColumnName("code").HasMaxLength(20).IsRequired();
                e.Property(x => x.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
                e.HasIndex(x => x.Code).IsUnique();
            });

            modelBuilder.Entity<FeeConcepto>(e =>
            {
                e.ToTable("fee_conceptos");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.Code).HasColumnName("code").HasMaxLength(20).IsRequired();
                e.Property(x => x.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
                e.HasIndex(x => x.Code).IsUnique();
            });

            modelBuilder.Entity<PaymentMethod>(e =>
            {
                e.ToTable("payment_methods");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.Code).HasColumnName("code").HasMaxLength(20).IsRequired();
                e.Property(x => x.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
                e.HasIndex(x => x.Code).IsUnique();
            });

            // ── Clients ────────────────────────────────────────────────────
            modelBuilder.Entity<Client>(e =>
            {
                e.ToTable("clients");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
                e.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
                e.Property(x => x.ClientTypeId).HasColumnName("client_type_id").IsRequired();
                e.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(150).IsRequired();
                e.Property(x => x.Rut).HasColumnName("rut").HasMaxLength(20);
                e.Property(x => x.RazonSocial).HasColumnName("razon_social").HasMaxLength(200);
                e.Property(x => x.RutEmpresa).HasColumnName("rut_empresa").HasMaxLength(20);
                e.Property(x => x.RepresentanteLegal).HasColumnName("representante_legal").HasMaxLength(150);
                e.Property(x => x.ContactoNombre).HasColumnName("contacto_nombre").HasMaxLength(100);
                e.Property(x => x.ContactoTelefono).HasColumnName("contacto_telefono").HasMaxLength(30);
                e.Property(x => x.ContactoEmail).HasColumnName("contacto_email").HasMaxLength(150);
                e.Property(x => x.UserId).HasColumnName("user_id");
                e.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
                e.HasOne(x => x.ClientType).WithMany().HasForeignKey(x => x.ClientTypeId);
            });

            // ── Cases ──────────────────────────────────────────────────────
            modelBuilder.Entity<Case>(e =>
            {
                e.ToTable("cases");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
                e.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
                e.Property(x => x.Expediente).HasColumnName("expediente").HasMaxLength(100);
                e.Property(x => x.Caratulado).HasColumnName("caratulado").HasMaxLength(300);
                e.Property(x => x.MateriaId).HasColumnName("materia_id").IsRequired();
                e.Property(x => x.Juzgado).HasColumnName("juzgado").HasMaxLength(200);
                e.Property(x => x.StatusId).HasColumnName("status_id").IsRequired();
                e.Property(x => x.AssignedLawyerId).HasColumnName("assigned_lawyer_id");
                e.Property(x => x.ClientId).HasColumnName("client_id").IsRequired();
                e.Property(x => x.StartDate).HasColumnName("start_date");
                e.Property(x => x.ClosedDate).HasColumnName("closed_date");
                e.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
                e.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
                e.HasOne(x => x.Materia).WithMany().HasForeignKey(x => x.MateriaId);
                e.HasOne(x => x.Status).WithMany().HasForeignKey(x => x.StatusId);
                e.HasOne(x => x.Client).WithMany(c => c.Cases).HasForeignKey(x => x.ClientId);
            });

            // ── CaseParts ──────────────────────────────────────────────────
            modelBuilder.Entity<CasePart>(e =>
            {
                e.ToTable("case_parts");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
                e.Property(x => x.CaseId).HasColumnName("case_id").IsRequired();
                e.Property(x => x.RoleId).HasColumnName("role_id").IsRequired();
                e.Property(x => x.ClientTypeId).HasColumnName("client_type_id").IsRequired();
                e.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(150);
                e.Property(x => x.Rut).HasColumnName("rut").HasMaxLength(20);
                e.Property(x => x.Telefono).HasColumnName("telefono").HasMaxLength(30);
                e.Property(x => x.Email).HasColumnName("email").HasMaxLength(150);
                e.Property(x => x.RazonSocial).HasColumnName("razon_social").HasMaxLength(200);
                e.Property(x => x.RepresentanteLegal).HasColumnName("representante_legal").HasMaxLength(150);
                e.HasOne(x => x.Case).WithMany(c => c.Parts).HasForeignKey(x => x.CaseId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.Role).WithMany().HasForeignKey(x => x.RoleId);
                e.HasOne(x => x.ClientType).WithMany().HasForeignKey(x => x.ClientTypeId);
            });

            // ── BitacoraEntries ────────────────────────────────────────────
            modelBuilder.Entity<BitacoraEntry>(e =>
            {
                e.ToTable("bitacora_entries");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
                e.Property(x => x.CaseId).HasColumnName("case_id").IsRequired();
                e.Property(x => x.Date).HasColumnName("date").IsRequired();
                e.Property(x => x.EventTypeId).HasColumnName("event_type_id").IsRequired();
                e.Property(x => x.Description).HasColumnName("description");
                e.Property(x => x.VisibleToClient).HasColumnName("visible_to_client").HasDefaultValue(false);
                e.Property(x => x.CreatedBy).HasColumnName("created_by").IsRequired();
                e.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
                e.HasOne(x => x.Case).WithMany(c => c.BitacoraEntries).HasForeignKey(x => x.CaseId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.EventType).WithMany().HasForeignKey(x => x.EventTypeId);
            });

            // ── Attachments ────────────────────────────────────────────────
            modelBuilder.Entity<Attachment>(e =>
            {
                e.ToTable("attachments");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
                e.Property(x => x.BitacoraEntryId).HasColumnName("bitacora_entry_id").IsRequired();
                e.Property(x => x.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
                e.Property(x => x.Url).HasColumnName("url").IsRequired();
                e.Property(x => x.Size).HasColumnName("size");
                e.Property(x => x.MimeType).HasColumnName("mime_type").HasMaxLength(100);
                e.Property(x => x.UploadedAt).HasColumnName("uploaded_at").HasDefaultValueSql("NOW()");
                e.HasOne(x => x.BitacoraEntry).WithMany(b => b.Attachments).HasForeignKey(x => x.BitacoraEntryId).OnDelete(DeleteBehavior.Cascade);
            });

            // ── Hearings ───────────────────────────────────────────────────
            modelBuilder.Entity<Hearing>(e =>
            {
                e.ToTable("hearings");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
                e.Property(x => x.CaseId).HasColumnName("case_id").IsRequired();
                e.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
                e.Property(x => x.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
                e.Property(x => x.Date).HasColumnName("date").IsRequired();
                e.Property(x => x.Time).HasColumnName("time");
                e.Property(x => x.Juzgado).HasColumnName("juzgado").HasMaxLength(200);
                e.Property(x => x.StatusId).HasColumnName("status_id").IsRequired();
                e.Property(x => x.Notes).HasColumnName("notes");
                e.Property(x => x.AlertDaysBefore).HasColumnName("alert_days_before").HasDefaultValue((short)3);
                e.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
                e.HasOne(x => x.Case).WithMany(c => c.Hearings).HasForeignKey(x => x.CaseId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.Status).WithMany().HasForeignKey(x => x.StatusId);
            });

            // ── Fees ───────────────────────────────────────────────────────
            modelBuilder.Entity<Fee>(e =>
            {
                e.ToTable("fees");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
                e.Property(x => x.CaseId).HasColumnName("case_id").IsRequired();
                e.Property(x => x.ConceptoId).HasColumnName("concepto_id").IsRequired();
                e.Property(x => x.TotalAmount).HasColumnName("total_amount").HasColumnType("numeric(14,2)").IsRequired();
                e.Property(x => x.Currency).HasColumnName("currency").HasMaxLength(3).HasDefaultValue("CLP");
                e.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
                e.HasOne(x => x.Case).WithOne(c => c.Fee).HasForeignKey<Fee>(x => x.CaseId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.Concepto).WithMany().HasForeignKey(x => x.ConceptoId);
                e.HasIndex(x => x.CaseId).IsUnique();
            });

            // ── Payments ───────────────────────────────────────────────────
            modelBuilder.Entity<Payment>(e =>
            {
                e.ToTable("payments");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
                e.Property(x => x.FeeId).HasColumnName("fee_id").IsRequired();
                e.Property(x => x.Amount).HasColumnName("amount").HasColumnType("numeric(14,2)").IsRequired();
                e.Property(x => x.Date).HasColumnName("date").IsRequired();
                e.Property(x => x.MethodId).HasColumnName("method_id").IsRequired();
                e.Property(x => x.Note).HasColumnName("note");
                e.Property(x => x.ReceiptUrl).HasColumnName("receipt_url");
                e.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
                e.HasOne(x => x.Fee).WithMany(f => f.Payments).HasForeignKey(x => x.FeeId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.Method).WithMany().HasForeignKey(x => x.MethodId);
            });
        }
    }
}

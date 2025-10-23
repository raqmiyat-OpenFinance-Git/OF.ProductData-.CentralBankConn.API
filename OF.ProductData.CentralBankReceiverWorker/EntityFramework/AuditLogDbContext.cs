using OF.ProductData.Model.EFModel.Audit;

namespace OF.ProductData.CoreBankConn.API.EFModel;

public class AuditLogDbContext : DbContext
{
    public AuditLogDbContext(DbContextOptions<AuditLogDbContext> options) : base(options) { }

    public DbSet<AuditLog> auditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

    }
}
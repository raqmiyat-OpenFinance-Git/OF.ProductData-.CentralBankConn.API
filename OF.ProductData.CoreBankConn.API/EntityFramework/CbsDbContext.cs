
using Microsoft.EntityFrameworkCore;
using OF.ProductData.Model.EFModel;

namespace OF.ProductData.CoreBankConn.API.EFModel;

public class CbsDbContext : DbContext
{
    public CbsDbContext(DbContextOptions<CbsDbContext> options) : base(options) { }
    public DbSet<CoreBankEnquiry> CoreBankEnquiries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Fluent API configs (optional)
    }
    
}
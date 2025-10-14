using OF.ProductData.Model.EFModel.Audit;
using OF.ProductData.Model.EFModel.Products;

namespace OF.ProductData.CoreBankConn.API.EFModel;

public class AuditLogDbContext : DbContext
{
    public AuditLogDbContext(DbContextOptions<AuditLogDbContext> options) : base(options) { }

    public DbSet<AuditLog> auditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //modelBuilder.Entity<ProductResponse>().ToTable("Products");

        //modelBuilder.Entity<CurrentAccounts>().ToTable("CurrentAccount");
        //modelBuilder.Entity<SavingsAccount>().ToTable("SavingsAccount");
        //modelBuilder.Entity<CreditCard>().ToTable("CreditCard");
        //modelBuilder.Entity<PersonalLoan>().ToTable("PersonalLoan");
        //modelBuilder.Entity<Mortgage>().ToTable("Mortgage");
        //modelBuilder.Entity<ProfitSharingRate>().ToTable("ProfitSharingRate");
        //modelBuilder.Entity<FinanceProfitRate>().ToTable("FinanceProfitRate");

        //// Relationships

        //modelBuilder.Entity<ProductResponse>()
        //    .HasMany(pr => pr.CurrentAccount)
        //    .WithOne(cr => cr.Product)
        //    .HasForeignKey(cr => cr.RequestId)
        //    .OnDelete(DeleteBehavior.Cascade);

        //modelBuilder.Entity<ProductResponse>()
        //    .HasMany(pr => pr.SavingsAccount)
        //    .WithOne(pa => pa.Product)
        //    .HasForeignKey(pa => pa.RequestId)
        //    .OnDelete(DeleteBehavior.Cascade);

        //modelBuilder.Entity<ProductResponse>()
        //    .HasMany(pr => pr.CreditCard)
        //    .WithOne(pii => pii.Product)
        //    .HasForeignKey(pii => pii.RequestId)
        //    .OnDelete(DeleteBehavior.Cascade);

        //modelBuilder.Entity<ProductResponse>()
        //    .HasMany(pr => pr.PersonalLoan)
        //    .WithOne(tpp => tpp.Product)
        //    .HasForeignKey(tpp => tpp.RequestId)
        //    .OnDelete(DeleteBehavior.Cascade);

        //modelBuilder.Entity<ProductResponse>()
        //   .HasMany(pr => pr.Mortgage)
        //   .WithOne(pa => pa.Product)
        //   .HasForeignKey(pa => pa.RequestId)
        //   .OnDelete(DeleteBehavior.Cascade);

        //modelBuilder.Entity<ProductResponse>()
        //   .HasMany(pr => pr.ProfitSharingRate)
        //   .WithOne(pa => pa.Product)
        //   .HasForeignKey(pa => pa.RequestId)
        //   .OnDelete(DeleteBehavior.Cascade);


        //modelBuilder.Entity<ProductResponse>()
        //   .HasMany(pr => pr.FinanceProfitRate)
        //   .WithOne(pa => pa.Product)
        //   .HasForeignKey(pa => pa.RequestId)
        //   .OnDelete(DeleteBehavior.Cascade);

    }
}
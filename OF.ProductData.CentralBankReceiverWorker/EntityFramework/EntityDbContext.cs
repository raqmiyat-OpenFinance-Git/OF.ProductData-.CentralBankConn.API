using OF.ProductData.Model.EFModel.CreateLead;
using OF.ProductData.Model.EFModel.Products;

namespace OF.ProductData.CoreBankConn.API.EFModel;

public class EntityDbContext : DbContext
{
    public EntityDbContext(DbContextOptions<EntityDbContext> options) : base(options) { }
    public DbSet<EFProductRequest>? ProductRequest { get; set; }

    public DbSet<EFProductResponse>? ProductResponse { get; set; }
    public DbSet<SavingsAccount> SavingsAccounts { get; set; }
    public DbSet<CurrentAccounts> CurrentAccounts { get; set; }
    public DbSet<CreditCard> CreditCards { get; set; }
    public DbSet<Mortgage> Mortgages { get; set; }
    public DbSet<ProfitSharingRate> ProfitSharingRate { get; set; }
    public DbSet<Finance> Finance { get; set; }
    public DbSet<DepositRates> DepositRates{ get; set; }
    public DbSet<FinanceRates> FinanceRates { get; set; }
    public DbSet<Tenor> Tenor { get; set; }
    public DbSet<AssetBacked> AssetBacked { get; set; }
    public DbSet<RewardsBenefits> RewardsBenefits { get; set; }
    public DbSet<EFCreateLeadRequest>? createLeadRequest { get; set; }
    public DbSet<EFCreateLeadResponse>? createLeadResponse { get; set; }
    public DbSet<EFCreateLeadHeaderRequest>? createLeadHeaderRequest { get; set; }
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

          
            modelBuilder.Entity<CurrentAccounts>()
                .HasOne(c => c.ProductRequest)
                .WithMany()
                .HasForeignKey(c => c.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SavingsAccount>()
                .HasOne(c => c.ProductRequest)
                .WithMany()
                .HasForeignKey(c => c.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CreditCard>()
                .HasOne(c => c.ProductRequest)
                .WithMany()
                .HasForeignKey(c => c.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Finance>()
                .HasOne(c => c.ProductRequest)
                .WithMany()
                .HasForeignKey(c => c.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Mortgage>()
                .HasOne(c => c.ProductRequest)
                .WithMany()
                .HasForeignKey(c => c.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProfitSharingRate>()
                .HasOne(c => c.ProductRequest)
                .WithMany()
                .HasForeignKey(c => c.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FinanceRates>()
                .HasOne(c => c.ProductRequest)
                .WithMany()
                .HasForeignKey(c => c.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }

    public class LeadDbContext : DbContext
    {
        public LeadDbContext(DbContextOptions<LeadDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<EFCreateLeadHeaderRequest>()
                .HasOne(c => c.CreateLeadRequest)
                .WithMany()
                .HasForeignKey(c => c.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

         

        }
    }

}
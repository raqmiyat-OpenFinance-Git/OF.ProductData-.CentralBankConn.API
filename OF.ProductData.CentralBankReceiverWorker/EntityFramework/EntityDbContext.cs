using OF.ProductData.Model.EFModel.Products;

namespace OF.ProductData.CoreBankConn.API.EFModel;

public class EntityDbContext : DbContext
{
    public EntityDbContext(DbContextOptions<EntityDbContext> options) : base(options) { }
    public DbSet<EFProductRequest>? ProductRequest { get; set; }

    public DbSet<EFProductResponse>? ProductResponse { get; set; }


    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options)
            : base(options) { }

    
        public DbSet<SavingsAccount> SavingsAccounts { get; set; }
        public DbSet<CurrentAccounts> CurrentAccounts { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }
        public DbSet<PersonalLoan> PersonalLoans { get; set; }
        public DbSet<Mortgage> Mortgages { get; set; }
        public DbSet<ProfitSharingRate> ProfitSharingRate { get; set; }
        public DbSet<FinanceProfitRate> FinanceProfitRate { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<EFProductResponse>().ToTable("Products");

            modelBuilder.Entity<CurrentAccounts>().ToTable("CurrentAccount");
            modelBuilder.Entity<SavingsAccount>().ToTable("SavingsAccount");
            modelBuilder.Entity<CreditCard>().ToTable("CreditCard");
            modelBuilder.Entity<PersonalLoan>().ToTable("PersonalLoan");
            modelBuilder.Entity<Mortgage>().ToTable("Mortgage");
            modelBuilder.Entity<ProfitSharingRate>().ToTable("ProfitSharingRate");
            modelBuilder.Entity<FinanceProfitRate>().ToTable("FinanceProfitRate");

            // Relationships
         

            modelBuilder.Entity<EFProductResponse>()
                .HasMany(pr => pr.CurrentAccount)
                .WithOne(cr => cr.Product)
                .HasForeignKey(cr => cr.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EFProductResponse>()
                .HasMany(pr => pr.SavingsAccount)
                .WithOne(pa => pa.Product)
                .HasForeignKey(pa => pa.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EFProductResponse>()
                .HasMany(pr => pr.CreditCard)
                .WithOne(pii => pii.Product)
                .HasForeignKey(pii => pii.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EFProductResponse>()
                .HasMany(pr => pr.PersonalLoan)
                .WithOne(tpp => tpp.Product)
                .HasForeignKey(tpp => tpp.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EFProductResponse>()
               .HasMany(pr => pr.Mortgage)
               .WithOne(pa => pa.Product)
               .HasForeignKey(pa => pa.RequestId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EFProductResponse>()
               .HasMany(pr => pr.ProfitSharingRate)
               .WithOne(pa => pa.Product)
               .HasForeignKey(pa => pa.RequestId)
               .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<EFProductResponse>()
               .HasMany(pr => pr.FinanceProfitRate)
               .WithOne(pa => pa.Product)
               .HasForeignKey(pa => pa.RequestId)
               .OnDelete(DeleteBehavior.Cascade);

        }
    }




}
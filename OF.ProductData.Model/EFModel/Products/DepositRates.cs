namespace OF.ProductData.Model.EFModel.Products
{
    [Table("Lfi_DepositRates")]
    public class DepositRates
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid DepositId { get; set; }

        [ForeignKey(nameof(ProductRequest))]
        public long RequestId { get; set; }

        public string? RateType { get; set; }

        public string? RateCategory { get; set; }
        public decimal? AnnualRate { get; set; }

        public decimal? AnnualMinRate { get; set; }
        public decimal? AnnualMaxRate { get; set; }

        public string? TierMinBalance { get; set; }
        public string? TierMaxBalance { get; set; }
        public string? TierCurrency { get; set; }

        public string? Term { get; set; } 
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public string? CalculationMethod { get; set; }
        public string? CalculationFrequency { get; set; }
        public string? ApplicationFrequency { get; set; }

        public string? Notes { get; set; }

        // Navigation
        public virtual EFProductRequest? ProductRequest { get; set; }
    }
}
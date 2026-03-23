
namespace OF.ProductData.Model.EFModel.Products
{
    [Table("Lfi_DepositRates")]
    public class DepositRatesModel
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey(nameof(ProductRequest))]
        public long RequestId { get; set; }

        public string? RateType { get; set; }

        // RateDetails properties
        public string? RateCategory { get; set; }
        public decimal? AnnualRate { get; set; }

        // AnnualRateRange properties
        public decimal? AnnualRateRangeMin { get; set; }
        public decimal? AnnualRateRangeMax { get; set; }

        // Tier properties
        public string? TierMinBalance { get; set; }
        public string? TierMaxBalance { get; set; }
        public string? TierCurrency { get; set; }

        // Term
        public string? Term { get; set; }

        // Dates
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }

        // Frequencies and methods
        public string? CalculationMethod { get; set; }
        public string? CalculationFrequency { get; set; }
        public string? ApplicationFrequency { get; set; }

        // Notes
        public string? Notes { get; set; }


        public virtual EFProductRequest? ProductRequest { get; set; }  // navigation

    }
}

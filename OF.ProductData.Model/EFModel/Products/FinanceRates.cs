namespace OF.ProductData.Model.EFModel.Products
{

    [Table("Lfi_FinanceRates")]
    public class FinanceRates
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey(nameof(ProductRequest))]
        public long RequestId { get; set; }

        // Basic properties
        public string? Description { get; set; }
        public decimal? Rate { get; set; }
        public string? BenchMark { get; set; }
        public decimal? BenchMarkRate { get; set; }
        public decimal? Margin { get; set; }

        // Rate review
        public string? RateReviewFrequency { get; set; }
        public DateTime? RateReviewNextDate { get; set; }

        // Frequencies and methods
        public string? CalculationFrequency { get; set; }
        public string? ApplicationFrequency { get; set; }
        public string? InterestCalculationMethod { get; set; }

        // AnnualPercentageRate properties
        public string? AnnualPercentageRateStartingFrom { get; set; }
        public decimal? AnnualPercentageRateUpTo { get; set; }
        public string? AnnualPercentageRateAdditionalInfo { get; set; }

        // Tier 1 properties
        public string? Tier1Name { get; set; }
        public string? Tier1Unit { get; set; }
        public string? Tier1ApplicationMethod { get; set; }

        // Tier 1 Balance Tier Details
        public string? Tier1BalanceMinAmount { get; set; }
        public string? Tier1BalanceMinCurrency { get; set; }
        public string? Tier1BalanceMaxAmount { get; set; }
        public string? Tier1BalanceMaxCurrency { get; set; }
        public decimal? Tier1BalanceRate { get; set; }

        // Tier 1 LTV Tier Details
        public decimal? Tier1LTVStart { get; set; }
        public decimal? Tier1LTVEnd { get; set; }
        public decimal? Tier1LTVRate { get; set; }

        // Tier 1 Rate Range
        public decimal? Tier1RateRangeMin { get; set; }
        public decimal? Tier1RateRangeMax { get; set; }
        public string? Tier1RateRangeAdditionalInfo { get; set; }

        // Conditions
        public string? Condition1Field { get; set; }
        public string? Condition1Operator { get; set; }
        public string? Condition1Value { get; set; }
        public string? Condition1Description { get; set; }

        // Notes
        public string? Notes { get; set; }

        // Additional Information
        public string? AdditionalInfo1Type { get; set; }
        public string? AdditionalInfo1Description { get; set; }

        // RateType
        public string? RateType { get; set; }

        public virtual EFProductRequest? ProductRequest { get; set; }  // navigation

    }
}


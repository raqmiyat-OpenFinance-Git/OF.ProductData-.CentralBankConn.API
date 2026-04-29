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
        public string? FinanceRateType { get; set; }
        public string? RateOption_RateType { get; set; }
        public string? Notes { get; set;}
        public string? APR_StartingFrom { get; set; }
        public decimal? APR_UpTo { get; set; }
        public string? APR_AdditionalInformation { get; set; }

        // 🔹 Tier
        public string? Tier_Name { get; set; }
        public string? Tier_Unit { get; set; }
        public string? Tier_ApplicationMethod { get; set; }

        // 🔹 Balance Tier
        public string? Balance_MinAmount { get; set; }
        public string? Balance_MinCurrency { get; set; }
        public string? Balance_MaxAmount { get; set; }
        public string? Balance_MaxCurrency { get; set; }
        public decimal? Balance_TierRate { get; set; }

        // 🔹 LTV
        public decimal? LTV_Start { get; set; }
        public decimal? LTV_End { get; set; }
        public decimal? LTV_TierRate { get; set; }

        // 🔹 Rate Range
        public decimal? RateRange_MinRate { get; set; }
        public decimal? RateRange_MaxRate { get; set; }
        public string? RateRange_AdditionalInformation { get; set; }

        // 🔹 Condition
        public string? Condition_Field { get; set; }
        public string? Condition_Operator { get; set; }
        public string? Condition_Value { get; set; }
        public string? Condition_Description { get; set; }

        // 🔹 Additional Info
        public string? AddInfo_Type { get; set; }
        public string? AddInfo_Description { get; set; }

        // ================= FIXED =================
        public string? Fixed_Description { get; set; }
        public decimal? Fixed_Rate { get; set; }
        public DateTime? Fixed_EndDate { get; set; }
        public string? Fixed_CalculationFrequency { get; set; }
        public string? Fixed_ApplicationFrequency { get; set; }
        public string? Fixed_ProfitCalculationMethod { get; set; }
        public string? FixedRateEnd { get; set; }

        public string? Fixed_APR_StartingFrom { get; set; }
        public decimal? Fixed_APR_UpTo { get; set; }
        public string? Fixed_APR_AdditionalInformation { get; set; }

        public string? Fixed_Tier_Name { get; set; }
        public string? Fixed_Tier_Unit { get; set; }
        public string? Fixed_Tier_ApplicationMethod { get; set; }

        public string? Fixed_Balance_MinAmount { get; set; }
        public string? Fixed_Balance_MinCurrency { get; set; }
        public string? Fixed_Balance_MaxAmount { get; set; }
        public string? Fixed_Balance_MaxCurrency { get; set; }
        public decimal? Fixed_Balance_TierRate { get; set; }

        public decimal? Fixed_LTV_Start { get; set; }
        public decimal? Fixed_LTV_End { get; set; }
        public decimal? Fixed_LTV_TierRate { get; set; }

        public decimal? Fixed_RateRange_MinRate { get; set; }
        public decimal? Fixed_RateRange_MaxRate { get; set; }
        public string? Fixed_RateRange_AdditionalInformation { get; set; }

        public string? Fixed_Condition_Field { get; set; }
        public string? Fixed_Condition_Operator { get; set; }
        public string? Fixed_Condition_Value { get; set; }
        public string? Fixed_Condition_Description { get; set; }

        public string? Fixed_AddInfo_Type { get; set; }
        public string? Fixed_AddInfo_Description { get; set; }

        // 🔹 Intro Period
        public string? Intro_Period { get; set; }
        public decimal? Intro_Indicative_Start { get; set; }
        public decimal? Intro_Indicative_End { get; set; }

        // ================= VARIABLE =================
        public string? Variable_Description { get; set; }
        public decimal? Variable_Rate { get; set; }
        public string? Variable_Benchmark { get; set; }
        public decimal? Variable_BenchmarkRate { get; set; }
        public decimal? Variable_Margin { get; set; }
        public string? Variable_ReviewFrequency { get; set; }
        public DateTime? Variable_ReviewNextDate { get; set; }

        public string? Variable_CalculationFrequency { get; set; }
        public string? Variable_ApplicationFrequency { get; set; }
        public string? Variable_ProfitCalculationMethod { get; set; }
        public string? Variable_Term { get; set; }

        public string? Variable_APR_StartingFrom { get; set; }
        public decimal? Variable_APR_UpTo { get; set; }
        public string? Variable_APR_AdditionalInformation { get; set; }

        public string? Variable_Tier_Name { get; set; }
        public string? Variable_Tier_Unit { get; set; }
        public string? Variable_Tier_ApplicationMethod { get; set; }

        public string? Variable_Balance_MinAmount { get; set; }
        public string? Variable_Balance_MinCurrency { get; set; }
        public string? Variable_Balance_MaxAmount { get; set; }
        public string? Variable_Balance_MaxCurrency { get; set; }
        public decimal? Variable_Balance_TierRate { get; set; }

        public decimal? Variable_LTV_Start { get; set; }
        public decimal? Variable_LTV_End { get; set; }
        public decimal? Variable_LTV_TierRate { get; set; }

        public decimal? Variable_RateRange_MinRate { get; set; }
        public decimal? Variable_RateRange_MaxRate { get; set; }
        public string? Variable_RateRange_AdditionalInformation { get; set; }

        public string? Variable_Condition_Field { get; set; }
        public string? Variable_Condition_Operator { get; set; }
        public string? Variable_Condition_Value { get; set; }
        public string? Variable_Condition_Description { get; set; }

        public string? Variable_AddInfo_Type { get; set; }
        public string? Variable_AddInfo_Description { get; set; }

        public virtual EFProductRequest? ProductRequest { get; set; }  // navigation

    }
}


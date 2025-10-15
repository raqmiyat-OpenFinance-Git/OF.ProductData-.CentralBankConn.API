namespace OF.ProductData.Model.EFModel.Products
{
    [Table("Mortgage")]
    public class Mortgage
    {

        [Key]
        public Guid Id { get; set; }
       [ForeignKey(nameof(Product))]
        public long RequestId { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }
       
        public decimal? MinimumLoanAmount { get; set; }
        public decimal? MaximumLoanAmount { get; set; }
        public decimal? MinTenure { get; set; }
        public decimal? MaxTenure { get; set; }      
        public string? CalculationMethod { get; set; }     
        public string? Structure { get; set; }     
        public string? RateType { get; set; }
        public string? RateDescription { get; set; }
        public string? ReviewFrequency { get; set; }
        public decimal? IndicativeRateFrom { get; set; }
        public decimal? IndicativeRateTo { get; set; }
        public decimal? ProfitRateFrom { get; set; }
        public decimal? ProfitRateTo { get; set; }
        public decimal? APRFrom { get; set; }
        public decimal? APRTo { get; set; }

        public decimal AnnualPercentageRateFrom { get; set; }
        public decimal AnnualPercentageRateTo { get; set; }
        public string? FixedRatePeriod { get; set; }
        public string? DebtBurdenRatio { get; set; }
        public string? DocumentationType { get; set; }
        public string? DocumentationDescription { get; set; }
        public string? FeaturesType { get; set; }
        public string? FeaturesDescription { get; set; }
        public string? FeesType { get; set; }
        public string? FeesPeriod { get; set; }
        public string? FeesName { get; set; }
        public string? FeesDescription { get; set; }
        public string? FeesUnit { get; set; }
        public decimal? FeesAmount { get; set; }
        public string? FeesCurrency { get; set; }
        public decimal? FeesPercentage { get; set; }
        public decimal? FeesUnitValue { get; set; }
        public decimal? FeesMaximumUnitValue { get; set; }
        public string? LimitsType { get; set; }
        public string? LimitsDescription { get; set; }
        public decimal? LimitsValue { get; set; }
        public string? BenefitsType { get; set; }
        public string? BenefitsName { get; set; }
        public string? BenefitsDescription { get; set; }
        public decimal? BenefitsValue { get; set; }
        public string? AdditionalInfoType { get; set; }
        public string? AdditionalInfoDescription { get; set; }

        // Navigation
        public virtual ProductResponse? Product { get; set; }
    }

}

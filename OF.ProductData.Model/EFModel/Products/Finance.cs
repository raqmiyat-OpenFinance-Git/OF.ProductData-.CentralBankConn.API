namespace OF.ProductData.Model.EFModel.Products
{
    [Table("Lfi_Finance")]
    public class Finance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [ForeignKey(nameof(ProductRequest))]
        public long RequestId { get; set; }
        public string? Type { get; set; }

        public decimal? MinimumFinanceAmount { get; set; }
        public string? MinimumFinanceCurrency { get; set; }

        public decimal? MaximumFinanceAmount { get; set; }
        public string? MaximumFinanceCurrency { get; set; }
        public string? DocumentationType { get; set; }
        public string? DocumentationDescription { get; set; }

        public string? FeaturesType { get; set; }
        public string? FeaturesDescription { get; set; }

        public string? ChargesType { get; set; }
        public string? ChargesDescription { get; set; }
        public string? ChargesName { get; set; }

        public decimal? ChargeAmount { get; set; }
        public string? ChargeCurrency { get; set; }
        public decimal? ChargeRate { get; set; }

        public string? ChargeApplicationFrequency { get; set; }
        public string? ChargeInterestCalculationMethod { get; set; }

        public decimal? MaximumChargeAmount { get; set; }
        public string? MaximumChargeCurrency { get; set; }

        public string? ChargeBasis { get; set; }

        public string? ConditionsField { get; set; }
        public string? ConditionsOperator { get; set; }
        public string? ConditionsValue { get; set; }
        public string? ConditionsDescription { get; set; }

        public string? Justification { get; set; }
        public string? Frequency { get; set; }

        public bool? DonatedToCharity { get; set; }

        public string? Notes { get; set; }
        public string? SupplementaryInformation { get; set; }

        public string? LimitsType { get; set; }
        public string? LimitsDescription { get; set; }

        public decimal? LimitsValue { get; set; }
        public decimal? LimitsAmount { get; set; }
        public string? LimitsCurrency { get; set; }

        public string? AdditionalInfoType { get; set; }
        public string? AdditionalInfoDescription { get; set; }

        // Navigation property
        public virtual EFProductRequest? ProductRequest { get; set; }
    }
}
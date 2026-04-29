namespace OF.ProductData.Model.EFModel.Products
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Lfi_Mortgage")]
    public class Mortgage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [ForeignKey(nameof(ProductRequest))]
        public long RequestId { get; set; }

        // Finance Amounts
        public decimal? MinimumFinanceAmount { get; set; }
        public string? MinimumFinanceCurrency { get; set; }

        public decimal? MaximumFinanceAmount { get; set; }
        public string? MaximumFinanceCurrency { get; set; }

        // Down Payment
        public string? DownPaymentCustomerCategory { get; set; }
        public decimal? DownPaymentMinimumPercent { get; set; }
        public string? DownPaymentBasis { get; set; }

        // Documentation
        public string? DocumentationType { get; set; }
        public string? DocumentationDescription { get; set; }

        // Features
        public string? FeaturesType { get; set; }
        public string? FeaturesDescription { get; set; }

        // Charges
        public string? ChargesType { get; set; }
        public string? ChargesName { get; set; }
        public string? ChargesDescription { get; set; }

        public decimal? ChargeAmount { get; set; }
        public string? ChargeCurrency { get; set; }
        public decimal? ChargeRate { get; set; }

        public string? ChargeApplicationFrequency { get; set; }
        public string? ChargeInterestCalculationMethod { get; set; }

        public decimal? MaximumChargeAmount { get; set; }
        public string? MaximumChargeCurrency { get; set; }

        public string? ChargeBasis { get; set; }

        // Charge Conditions
        public string? ConditionsField { get; set; }
        public string? ConditionsOperator { get; set; }
        public string? ConditionsValue { get; set; }
        public string? ConditionsDescription { get; set; }

        public string? Justification { get; set; }
        public string? Frequency { get; set; }

        public bool? DonatedToCharity { get; set; }

        public string? Notes { get; set; }
        public string? SupplementaryInformation { get; set; }

        // Limits
        public string? LimitsType { get; set; }
        public string? LimitsDescription { get; set; }

        public decimal? LimitsAmount { get; set; }
        public string? LimitsCurrency { get; set; }

        public decimal? LimitsValue { get; set; }
        public decimal? LimitsPercentage { get; set; }

        // Navigation Property
        public virtual EFProductRequest? ProductRequest { get; set; }
    }
}
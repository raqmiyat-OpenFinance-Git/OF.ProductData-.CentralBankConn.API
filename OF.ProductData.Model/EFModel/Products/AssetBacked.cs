namespace OF.ProductData.Model.EFModel.Products
{
    [Table("Lfi_AssetBacked")]
    public class AssetBacked
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey(nameof(ProductRequest))]
        public long RequestId { get; set; }
        public string? Type { get; set; }
        public string? AssetType { get; set; }
        public string? Description { get; set; }

        // Valuation properties (flattened)
        public DateTime? ValuationDate { get; set; }
        public decimal? ValuationAmount { get; set; }
        public string? ValuationCurrency { get; set; }

        // SupplementaryInformation properties (flattened)
        public string? SupplementaryInformationData { get; set; } // Store as JSON or string

        // OwnershipTransfer properties (flattened)
        public DateTime? TransferOfOwnershipDate { get; set; }
        public string? OwnershipTransferType { get; set; }
        public string? OwnershipTransferMethod { get; set; }

        // TokenPurchaseAmount properties (flattened)
        public decimal? TokenPurchaseAmount { get; set; }
        public string? TokenPurchaseCurrency { get; set; }

        // BuyoutSchedule properties (flattened)
        public string? BuyoutScheduleFrequency { get; set; }
        public decimal? BuyoutScheduleAmount { get; set; }
        public string? BuyoutScheduleCurrency { get; set; }

        // SaleAgreement properties (flattened)
        public bool? SaleAgreementRequired { get; set; }
        public string? SaleAgreementExecution { get; set; }
        public decimal? SaleAgreementPrice { get; set; }
        public string? SaleAgreementPriceCurrency { get; set; }

        // TransferConditions (store as comma-separated string or JSON)
        public string? TransferConditions { get; set; }

        public virtual EFProductRequest? ProductRequest { get; set; }  // navigation
    }
}

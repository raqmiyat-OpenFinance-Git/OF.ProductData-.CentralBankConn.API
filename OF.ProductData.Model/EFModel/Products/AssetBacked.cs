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

        public DateTime? ValuationDate { get; set; }
        public string? ValuationAmount { get; set; }
        public string? ValuationCurrency { get; set; }

        public string? SupplementaryInformation { get; set; }

        public DateTime? TransferOfOwnershipDate { get; set; }
        public string? OwnershipType { get; set; }
        public string? OwnershipMethod { get; set; }

        public string? TokenPurchaseAmount { get; set; }
        public string? TokenPurchaseCurrency { get; set; }

        public string? BuyoutFrequency { get; set; }
        public string? BuyoutAmount { get; set; }
        public string? BuyoutCurrency { get; set; }

        public bool? SaleRequired { get; set; }
        public string? SaleExecution { get; set; }

        public string? SalePriceAmount { get; set; }
        public string? SalePriceCurrency { get; set; }

        public string? TransferCondition { get; set; }

        public virtual EFProductRequest? ProductRequest { get; set; }  // navigation
    }
}

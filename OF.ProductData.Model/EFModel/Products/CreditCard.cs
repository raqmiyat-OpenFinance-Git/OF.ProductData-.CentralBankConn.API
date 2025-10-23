namespace OF.ProductData.Model.EFModel.Products
{
    [Table("Lfi_CreditCard")]
    public class CreditCard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey(nameof(ProductRequest))]
        public long RequestId { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }
        public decimal? Rate { get; set; }
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
        public virtual EFProductRequest? ProductRequest { get; set; }  // navigation
    }

}

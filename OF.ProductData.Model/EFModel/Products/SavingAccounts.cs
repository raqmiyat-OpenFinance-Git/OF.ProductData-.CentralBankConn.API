namespace OF.ProductData.Model.EFModel.Products
{
    [Table("SavingsAccount")]
    public class SavingsAccount
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(Product))]
        public long RequestId { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }

        public decimal? MinimumBalance { get; set; }
        public string? Currency { get; set; }

        public decimal? AnnualReturn { get; set; }
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

        public virtual ProductResponse? Product { get; set; }

    }

}

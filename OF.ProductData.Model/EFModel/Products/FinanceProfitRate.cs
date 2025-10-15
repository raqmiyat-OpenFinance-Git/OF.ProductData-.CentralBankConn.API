namespace OF.ProductData.Model.EFModel.Products
{
    [Table("FinanceProfitRate")]
    public class FinanceProfitRate
    {

        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(Product))]
        public long RequestId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? CalculationMethod { get; set; }
        public decimal? Rate { get; set; }
        public string? Frequency { get; set; }
        public string? TiersType { get; set; }
        public string? TiersDescription { get; set; }
        public string? TiersName { get; set; }
        public string? TiersUnit { get; set; }
        public decimal? TiersMinimumTierValue { get; set; }
        public decimal? TiersMaximumTierValue { get; set; }
        public string? TiersMinimumTierCurrency { get; set; }
        public string? TiersMaximumTierCurrency { get; set; }
        public decimal? TiersMinimumTierRate { get; set; }
        public decimal? TiersMaximumTierRate { get; set; }
        public string? TiersCondition { get; set; }
        public string? AdditionalInfoType { get; set; }
        public string? AdditionalInfoDescription { get; set; }
        public virtual EFProductResponse? Product { get; set; }
    }
}

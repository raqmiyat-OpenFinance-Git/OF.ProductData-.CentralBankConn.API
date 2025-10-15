namespace OF.ProductData.Model.EFModel.Products
{
    [Table("ProfitSharingRate")]
    public class ProfitSharingRate
    {

        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(Product))]
        public long RequestId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public decimal? MinimumDepositAmount { get; set; }
        public string? Currency { get; set; }

        public decimal? AnnualReturn { get; set; }


        public string? InvestmentPeriodName { get; set; }
        public string? InvestmentPeriodDescription { get; set; }


        public string? AnnualReturnName { get; set; }
        public string? AnnualReturnDescription { get; set; }


        public string? AdditionalInfoType { get; set; }
        public string? AdditionalInfoDescription { get; set; }


        public virtual EFProductResponse? Product { get; set; }
    }
}

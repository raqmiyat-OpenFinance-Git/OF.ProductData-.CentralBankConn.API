namespace OF.ProductData.Model.EFModel.Products
{

    [Table("Lfi_RewardsBenefits")]
    public  class RewardsBenefits
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey(nameof(ProductRequest))]
        public long RequestId { get; set; }
        // RewardsBenefits properties
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }

        // Balance properties (flattened)
        public decimal? BalanceAmount { get; set; }
        public string? BalanceCurrency { get; set; }

        // RewardBasis (store as comma-separated string or JSON)
        public string? RewardBasis { get; set; }

        // FrequencyPaid
        public string? FrequencyPaid { get; set; }
        public virtual EFProductRequest? ProductRequest { get; set; }  // navigation
    }
}

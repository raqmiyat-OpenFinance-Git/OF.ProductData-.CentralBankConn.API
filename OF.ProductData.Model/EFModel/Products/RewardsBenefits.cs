namespace OF.ProductData.Model.EFModel.Products
{

    [Table("Lfi_RewardsBenefits")]
    public  class RewardsBenefits
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid RewardId { get; set; }

        [ForeignKey(nameof(ProductRequest))]
        public long RequestId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public string? RewardBasis { get; set; }
        public virtual EFProductRequest? ProductRequest { get; set; } 
    }
}

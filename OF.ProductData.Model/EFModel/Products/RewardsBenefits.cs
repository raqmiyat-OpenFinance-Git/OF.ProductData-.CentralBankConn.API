using OF.ProductData.Model.CentralBank;
using OF.ProductData.Model.CentralBank.Products;

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
        public string? BalanceAmount { get; set; }
        public string? BalanceCurrency { get; set; }
        public string? FrequencyPaid { get; set; }
        public string? PointsType { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public virtual EFProductRequest? ProductRequest { get; set; } 
    }
}

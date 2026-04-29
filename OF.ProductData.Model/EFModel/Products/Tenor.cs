namespace OF.ProductData.Model.EFModel.Products
{
    [Table("Lfi_Tenor")]
    public class Tenor
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey(nameof(ProductRequest))]
        public long RequestId { get; set; }
        public string? MinimumTenor { get; set; }
        public string? MaximumTenor { get; set; }
        public string? Condition { get; set; }
        public virtual EFProductRequest? ProductRequest { get; set; }  // navigation

    }
}


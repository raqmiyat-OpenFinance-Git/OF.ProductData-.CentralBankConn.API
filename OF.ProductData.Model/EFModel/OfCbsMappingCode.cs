namespace OF.ProductData.Model.EFModel;

[Table("OfCbsMappingCodes")]
public class OfCbsMappingCode
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column(TypeName = "numeric(10, 0)")]
    public decimal? HttpStatus { get; set; }

    [MaxLength(100)]
    public string? CBStatusCode { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(200)]
    public string? CbsMappingCode { get; set; }
}
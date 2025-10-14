namespace OF.ProductData.Model.EFModel.Products;

[Table("ProductDataRequest")]
public class ProductRequest
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long RequestId { get; set; }  
    public Guid CorrelationId { get; set; }  
    public int? PageNumber { get; set; }  
    public int? PageSize { get; set; } 
    public string? ProductCategory { get; set; }  
    public bool? IsShariaCompliant { get; set; } 
    public string? LastUpdatedDateTime { get; set; }
    public string? SortOrder { get; set; } = "asc";  
    public string? SortField { get; set; }
    public string? Authorization { get; set; }  
    public string? CustomerIpAddress { get; set; }  
    public string? Status { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public string? RequestJson { get; set; }
}

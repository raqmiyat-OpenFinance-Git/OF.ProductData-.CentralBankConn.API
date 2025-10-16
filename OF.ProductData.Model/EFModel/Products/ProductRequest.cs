namespace OF.ProductData.Model.EFModel.Products;

[Table("Lfi_ProductDataRequest")]
public class EFProductRequest
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long RequestId { get; set; }
    public Guid CorrelationId { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
    public string? O3ProviderId { get; set; }
    public string? O3AspspId { get; set; }
    public string? O3CallerOrgId { get; set; }
    public string? O3CallerClientId { get; set; }
    public string? O3CallerSoftwareStatementId { get; set; }
    public string? O3ApiUri { get; set; }
    public string? O3CallerInteractionId { get; set; }
    public string? O3OzoneInteractionId { get; set; }
    public string? O3ApiOperation { get; set; }
    public string? ProductCategory { get; set; }
    public bool? IsShariaCompliant { get; set; }
    public string? LastUpdatedDateTime { get; set; }
    public string? SortOrder { get; set; } = "asc";
    public string? SortField { get; set; }
    public string? CustomerIpAddress { get; set; }
    public string? Status { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public string? RequestJson { get; set; }
}

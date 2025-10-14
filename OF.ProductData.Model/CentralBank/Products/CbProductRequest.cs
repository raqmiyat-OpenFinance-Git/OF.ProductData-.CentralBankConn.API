namespace OF.ProductData.Model.CentralBank.Products;

public class CbProductRequest
{
   
    [Required]
    public string Authorization { get; set; } = string.Empty;

    [Required]
    public string CustomerIpAddress { get; set; } = string.Empty;
    public string? ProductCategory { get; set; }
    public bool? IsShariaCompliant { get; set; }
    public DateTime? LastUpdatedDateTime { get; set; }

    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;
    [Range(1, 100)]
    public int PageSize { get; set; } = 100;

    public string SortOrder { get; set; } = "asc";

    public string SortField { get; set; } = "LastUpdatedDateTime";
    public Guid CorrelationId { get; set; }
    public string? ExternalRefNbr { get; set; }

    public string? RequestJson { get; set; }
}

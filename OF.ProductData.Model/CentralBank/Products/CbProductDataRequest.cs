using Microsoft.AspNetCore.Mvc;

namespace OF.ProductData.Model.CentralBank.Products;

public class CbProductDataRequest
{

  
    public string? O3ProviderId { get; set; }

  
    public string? O3AspspId { get; set; }

  
    public string? O3CallerOrgId { get; set; }

    
    public string? O3CallerClientId { get; set; }

    public string? O3CallerSoftwareStatementId { get; set; }

  
    public string? O3ApiUri { get; set; }

    public string? O3CallerInteractionId { get; set; }
    public string? O3OzoneInteractionId { get; set; }
    public string? O3ApiOperation { get; set; }
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

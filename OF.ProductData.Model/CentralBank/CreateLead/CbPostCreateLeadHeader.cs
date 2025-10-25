namespace OF.ProductData.Model.CentralBank.CreateLead;

public class CbPostCreateLeadHeader
{
    public Guid CorrelationId { get; set; }
    public long RequestId { get; set; }
    public string? O3ProviderId { get; set; }
    public string? O3CallerOrgId { get; set; }
    public string? O3CallerClientId { get; set; }
    public string? O3CallerSoftwareStatementId { get; set; }
    public string? O3ApiUri { get; set; }
    public string? O3ApiOperation { get; set; }
    public string? O3CallerInteractionId { get; set; }    
    public string? O3OzoneInteractionId { get; set; }
    public string? XFapiCustomerIpAddress { get; set; }



}

namespace OF.ProductData.Model.CoreBank.Products;

public class CbsCreateLeadRequest
{
    public string? ExternalRefNbr { get; set; }
    public string? ConsentId { get; set; }
    public string? accountId { get; set; }
    public Guid CorrelationId { get; set; }

}
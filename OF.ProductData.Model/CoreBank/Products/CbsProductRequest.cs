namespace OF.ProductData.Model.CoreBank.Products;

public class CbsProductRequest
{
    public string? ExternalRefNbr { get; set; }
    public string? ConsentId { get; set; }
    public string? accountId { get; set; }
    public Guid CorrelationId { get; set; }

}
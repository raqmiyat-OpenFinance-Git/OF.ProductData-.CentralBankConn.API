namespace OF.ProductData.Model.CentralBank.Products;

public class CbProductResponseWrapper
{
    //public Guid AccountId { get; set; }
    public Guid CorrelationId { get; set; }
    public string? AccountId { get; set; }
    public CbProductResponse? centralBankProductResponse { get; set; }
}


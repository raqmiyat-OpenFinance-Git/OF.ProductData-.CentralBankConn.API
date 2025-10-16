using OF.ProductData.Model.CoreBank.Products;

namespace OF.ProductData.Model.CentralBank.Products;

public class CbCreateLeadResponseWrapper
{
   
    public Guid CorrelationId { get; set; }
    public string? AccountId { get; set; }
    public CbsCreateLeadResponse? centralBankCreateLeadResponse { get; set; }
}


namespace OF.ProductData.Model.Common;

public class CoreBankApis
{
    public string? BaseUrl { get; set; }
    public string? TokenUrl { get; set; }

    public ProductServiceUrl? ProductServiceUrl { get; set; }
    public LeadServiceUrl? LeadServiceUrl { get; set; }
}



public class ProductServiceUrl
{
    public string? GetProductUrl { get; set; }
}
public class LeadServiceUrl
{
    public string? GetLeadUrl { get; set; }
}









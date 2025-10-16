using OF.ProductData.Model.CentralBank.Products;
using OF.ServiceInitiation.Model.CentralBank.Payments.PostHeader;

namespace OF.DataSharing.Model.CentralBank.CoPQuery;

public class CbPostCreateLeadRequestDto
{
    public Guid CorrelationId { get; set; }
    public CbPostCreateLeadHeader? cbPostCreateLeadHeader { get; set; }
    public CbPostCreateLeadRequest? cbPostCreateLeadRequest { get; set; }
}

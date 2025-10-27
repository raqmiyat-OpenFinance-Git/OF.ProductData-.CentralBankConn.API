using OF.ProductData.Model.CentralBank.CreateLead;

namespace OF.DataSharing.Model.CentralBank.CoPQuery;

public class CbPostCreateLeadRequestDto
{
    public Guid CorrelationId { get; set; }
    public CbPostCreateLeadHeader? cbPostCreateLeadHeader { get; set; }
    public CbPostCreateLeadRequest? cbPostCreateLeadRequest { get; set; }
}

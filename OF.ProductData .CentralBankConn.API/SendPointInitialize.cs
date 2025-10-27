namespace OF.ProductData.CentralBankConn.API.Models;

public class SendPointInitialize
{
 
    public ISendEndpoint? GetProductDataRequest { get; set; }
    public ISendEndpoint? GetProductDataResponse { get; set; }
    public ISendEndpoint? PostLeadRequest { get; set; }
    public ISendEndpoint? PostLeadResponse { get; set; }
    public ISendEndpoint? AuditLog { get; set; }
}

namespace OF.ProductData.CentralBankConn.API.Models;

public class SendPointInitialize
{
 
    public ISendEndpoint? GetProductDataRequest { get; set; }
    public ISendEndpoint? GetProductDataResponse { get; set; }
    public ISendEndpoint? AuditLog { get; set; }
}

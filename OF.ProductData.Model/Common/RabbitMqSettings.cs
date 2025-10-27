namespace OF.ProductData.Model.Common;

public class RabbitMqSettings
{
    public string? Url { get; set; }
    public string? UserName { get; set; }
    public string? Rabitphrase { get; set; }
    public bool IsEncrypted { get; set; }
    public int IdleTimeoutMilliSeconds { get; set; }
    public string? GetProductDataRequest { get; set; }
    public string? GetProductDataResponse { get; set; }
    public string? PostLeadRequest { get; set; }
    public string? PostLeadResponse { get; set; }
    public string? AuditLog { get; set; }
}


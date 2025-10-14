namespace OF.ProductData.Model.EFModel;

[Table("CoreBankEnquiry")]
public class CoreBankEnquiry
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public string RequestType { get; set; } = string.Empty;

    public string? ExternalReferenceNumber { get; set; }

    public string? Status { get; set; }

    public string AccountNumber { get; set; } = string.Empty;

    public string TransactionType { get; set; } = string.Empty; // "CR" or "DR"

    public decimal Amount { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public string? Module { get; set; }

    public string? ReturnCode { get; set; }

    public string? ReturnDescription { get; set; }

    public string? HostReferenceNumber { get; set; }

    public DateTime? MessageSentAt { get; set; }

    public DateTime? MessageReceivedAt { get; set; }

    public int? RetryCount { get; set; }

    public DateTime? RetryOn { get; set; }

    public string? RequestPayload { get; set; }

    public string? ResponsePayload { get; set; }

    public string? Comments { get; set; }

    public DateTime? LastUpdatedOn { get; set; }

}

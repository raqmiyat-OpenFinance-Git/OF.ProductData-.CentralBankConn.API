namespace OF.ProductData.Common.AES;

public class SecurityParameters
{
    public string? KeyValue { get; set; }
    public bool IsEncrypted { get; set; }
    public string? InitVector { get; set; }
    public string? EncryptionPharse { get; set; }
    public string? UserName { get; set; }
}

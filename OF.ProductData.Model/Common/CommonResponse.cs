namespace OF.ProductData.Model.Common;
public class CommonResponse
{
    public string? ResponseCode { get; set; }
    public string? ResponseContent { get; set; }
    public string? token_type { get; set; }
    public int expires_in { get; set; }
    public string? access_token { get; set; }
}

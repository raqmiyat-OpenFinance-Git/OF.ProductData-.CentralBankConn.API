namespace OF.ProductData.Model.Common;
public class JwtSettings
{
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public string? Key { get; set; }
    public int ExpiryInMinutes { get; set; }
    public bool EnableAuth { get; set; }
    public string? StaticToken { get; set; }
    public bool StaticTokenEnabled { get; set; }
    public List<ValidClientsList>? ValidClients { get; set; }
}
public class ValidClientsList
{
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
}


namespace OF.ProductData.Model.Common;

public class RedisCacheSettings
{
    public string? Url { get; set; }
    public bool EnableCache { get; set; }
    public int CacheExpirationMinutes { get; set; }
}


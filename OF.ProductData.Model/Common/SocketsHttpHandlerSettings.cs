namespace OF.ProductData.Model.Common;

public class SocketsHttpHandlerSettings
{
    public int HandlerLifetimeInHours { get; set; }
    public int ClientTimeoutInSeconds { get; set; }
    public int PooledConnectionLifetimeInHours { get; set; }
    public int PooledConnectionIdleTimeoutInMinutes { get; set; }
    public int KeepAlivePingDelayInMinutes { get; set; }
    public int KeepAlivePingTimeoutInSeconds { get; set; }
    public bool UseCookies { get; set; }
    public string? CertificateRevocationCheckMode { get; set; }
    public bool RemoteCertificateValidationCallback { get; set; }
    public string? EnabledSslProtocols { get; set; }
}
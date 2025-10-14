namespace OF.ProductData.Common.Custom;

public static class SslClient
{
    public static SslClientAuthenticationOptions AwakeSslVaidation(bool byPassSSL)
    {
        if (byPassSSL)
        {
            return new SslClientAuthenticationOptions
            {
                EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };
        }
        else
        {
            return new SslClientAuthenticationOptions
            {
                EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13
            };
        }
    }
    public static SslProtocols ParseSslProtocols(string protocolsString)
    {
        if (string.IsNullOrWhiteSpace(protocolsString))
        {
            throw new ArgumentException("Protocols string cannot be null or empty", nameof(protocolsString));
        }

        // Split the string by commas
        var protocols = protocolsString.Split(',')
            .Select(p => p.Trim()) // Remove any extra whitespace
            .Select(p => (SslProtocols)Enum.Parse(typeof(SslProtocols), p, ignoreCase: true))
            .Aggregate((current, next) => current | next); // Combine protocols using bitwise OR

        return protocols;
    }

    public static X509RevocationMode ParseRevocationMode(string revocationModeString)
    {
        if (string.IsNullOrWhiteSpace(revocationModeString))
        {
            throw new ArgumentException("Revocation mode string cannot be null or empty", nameof(revocationModeString));
        }

        // Convert the string to the corresponding X509RevocationMode enum value
        return (X509RevocationMode)Enum.Parse(typeof(X509RevocationMode), revocationModeString, ignoreCase: true);
    }
}

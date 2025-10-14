namespace OF.ProductData.Common.Custom;

public static class SqlConManager
{
    public static string GetConnectionString(string connectionString, bool isEncrypted, bool isEntityConnection, Logger logger)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));

        try
        {
            // Decrypt only if flagged
            var decryptedConnectionString = isEncrypted
                ? EncryptDecrypt.Decrypt(connectionString, logger)
                : connectionString;

            // Return as-is for Entity Framework connection strings
            if (isEntityConnection)
                return decryptedConnectionString;

            // For ADO.NET connections, ensure it's valid via builder
            var connectionStringBuilder = new SqlConnectionStringBuilder(decryptedConnectionString);
            return connectionStringBuilder.ToString();
        }
        catch (Exception ex)
        {
            logger.Error($"Exception at {nameof(SqlConManager)}-GetConnectionString(): {ex}");
            throw;
        }
    }


}

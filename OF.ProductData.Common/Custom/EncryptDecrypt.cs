namespace OF.ProductData.Common.Custom;

public static class EncryptDecrypt
{
    public static string Encrypt(string toEncrypt, Logger logger)
    {
        try
        {
            var key = "RyCt4d6Wl85N4u2T";

            byte[] keyArray;
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);

            keyArray = SHA256.HashData(Encoding.UTF8.GetBytes(key));

            using Aes aesAlg = Aes.Create();
            aesAlg.Key = keyArray;

            aesAlg.Mode = CipherMode.ECB;
            aesAlg.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = aesAlg.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        catch (Exception ex)
        {
            logger.Error($"Exception at {nameof(EncryptDecrypt)}-Encrypt(): {ex}");
            throw;
        }
    }
    public static string Decrypt(string toDecrypt, Logger logger)
    {
        try
        {
            var key = "RyCt4d6Wl85N4u2T";

            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

            keyArray = SHA256.HashData(Encoding.UTF8.GetBytes(key));

            using Aes aesAlg = Aes.Create();
            aesAlg.Key = keyArray;

            aesAlg.Mode = CipherMode.ECB;
            aesAlg.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = aesAlg.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }
        catch (Exception ex)
        {
            logger.Error($"Exception at {nameof(EncryptDecrypt)}-Decrypt(): {ex}");
            throw;
        }
    }
}

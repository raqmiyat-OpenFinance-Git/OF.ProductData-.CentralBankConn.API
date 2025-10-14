namespace OF.ProductData.Common.AES;

public class AesCbcGenericService
{
    public string Encrypt(string value, string key, string initVector, Logger logger)
    {
        try
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            using Aes aesAlg = Aes.Create();
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;
            aesAlg.Key = keyBytes;
            aesAlg.IV = initVectorBytes;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            byte[] encrypted;

            using (var msEncrypt = new MemoryStream())
            {
                using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
                using (var swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(value);
                }
                encrypted = msEncrypt.ToArray();
            }
            return Convert.ToBase64String(encrypted);
        }
        catch (Exception ex)
        {
            logger.Error("--------------------------Execpetion at {AesCbcGenericService}-Encrypt():--------------------------");
            logger.Error(ex);
            logger.Error("--------------------------Execpetion at {AesCbcGenericService}-Encrypt():--------------------------");
            throw new InvalidOperationException(ex.Message);
        }

    }

    public string Decrypt(string encryptedText, string key, string initVector, Logger logger)
    {
        try
        {
            byte[] encrypted = Convert.FromBase64String(encryptedText);
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            using Aes aesAlg = Aes.Create();
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;
            aesAlg.Key = keyBytes;
            aesAlg.IV = initVectorBytes;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using var msDecrypt = new MemoryStream(encrypted);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            return srDecrypt.ReadToEnd();
        }
        catch (Exception ex)
        {
            logger.Error("--------------------------Execpetion at {AesCbcGenericService}-Decrypt():--------------------------");
            logger.Error(ex);
            logger.Error("--------------------------Execpetion at {AesCbcGenericService}-Decrypt():--------------------------");
            throw new InvalidOperationException(ex.Message);
        }
    }
}

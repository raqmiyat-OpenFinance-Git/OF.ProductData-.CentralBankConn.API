namespace OF.ProductData.Model.Common;

public class DatabaseConfig
{
	public string? ConnectionString { get; set; }
	public int CommandTimeoutSeconds { get; set; }
	public bool UseEncryption { get; set; }
	public bool IsEntityFramework { get; set; }

}
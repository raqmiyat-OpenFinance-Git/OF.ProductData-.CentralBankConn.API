namespace OF.ProductData.Common.NLog;

public class ProductLogger : BaseLogger
{
    public ProductLogger(IConfiguration configuration) : base(configuration)
    {
        bool siemEnabled = configuration.GetValue<bool>("SIEM-Ready-Log");

        if (siemEnabled)
        {
            LogManager.Setup().LoadConfigurationFromFile("NLog.config");
            Log = LogManager.GetLogger("ProductJsonLogger");
        }
        else
        {
            Log = LogManager.GetLogger("ProductLogger");
        }
    }
}








namespace OF.ProductData.Common.NLog;

public class ApiClientLogger : BaseLogger
{
    public ApiClientLogger(IConfiguration configuration) : base(configuration)
    {
        bool siemEnabled = configuration.GetValue<bool>("SIEM-Ready-Log");

        if (siemEnabled)
        {
            LogManager.Setup().LoadConfigurationFromFile("NLog.config");
            Log = LogManager.GetLogger("ApiClientLoggerJson");
        }
        else
        {
            Log = LogManager.GetLogger("ApiClientLogger");
        }
    }
}




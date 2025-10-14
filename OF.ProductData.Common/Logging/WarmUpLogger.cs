namespace OF.ProductData.Common.NLog;

public class WarmUpLogger : BaseLogger
{
    public WarmUpLogger(IConfiguration configuration) : base(configuration)
    {
        bool siemEnabled = configuration.GetValue<bool>("SIEM-Ready-Log");

        if (siemEnabled)
        {
            LogManager.Setup().LoadConfigurationFromFile("NLog.config");
            Log = LogManager.GetLogger("WarmUpLoggerJson");
        }
        else
        {
            Log = LogManager.GetLogger("WarmUpLogger");
        }
    }
}




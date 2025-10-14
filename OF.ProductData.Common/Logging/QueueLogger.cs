namespace OF.ProductData.Common.NLog;

public class QueueLogger : BaseLogger
{
    public QueueLogger(IConfiguration configuration) : base(configuration)
    {
        bool siemEnabled = configuration.GetValue<bool>("SIEM-Ready-Log");

        if (siemEnabled)
        {
            LogManager.Setup().LoadConfigurationFromFile("NLog.config");
            Log = LogManager.GetLogger("QueueJsonLogger");
        }
        else
        {
            Log = LogManager.GetLogger("QueueLogger");
        }
    }
}








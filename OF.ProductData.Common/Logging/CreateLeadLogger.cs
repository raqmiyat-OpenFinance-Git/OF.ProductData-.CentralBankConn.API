namespace OF.ProductData.Common.NLog;

public class CreateLeadLogger : BaseLogger
{
    public CreateLeadLogger(IConfiguration configuration) : base(configuration)
    {
        bool siemEnabled = configuration.GetValue<bool>("SIEM-Ready-Log");

        if (siemEnabled)
        {
            LogManager.Setup().LoadConfigurationFromFile("NLog.config");
            Log = LogManager.GetLogger("CreateLeadJsonLogger");
        }
        else
        {
            Log = LogManager.GetLogger("CreateLeadLogger");
        }
    }
}








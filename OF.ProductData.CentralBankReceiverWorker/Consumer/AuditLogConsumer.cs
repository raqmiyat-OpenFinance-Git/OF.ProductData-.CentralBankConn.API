using OF.ProductData.CentralBankReceiverWorker.IServices;
using OF.ProductData.Common.NLog;
using OF.ProductData.Model.EFModel.Audit;

namespace OF.ProductData.CentralBankReceiverWorker.Consumer;

#region MassTransitConsumer
[ExcludeFromConfigureEndpoints]
public class AuditLogConsumer : IConsumer<AuditLog>
{
    private readonly ProductLogger _logger;
    private readonly IAuditLogger _auditLogger;
    public AuditLogConsumer(ProductLogger logger, IAuditLogger auditLogger)
    {
        _logger = logger;
        _auditLogger = auditLogger;
    }
    public async Task Consume(ConsumeContext<AuditLog> context)
    {
        try
        {
            if (context?.Message == null)
            {
                _logger.Warn("AuditLog message is null.");
                return;
            }
            await ProcessAsync(context.Message);

        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unhandled exception in AuditLog.Consume()");
        }
    }
    private async Task ProcessAsync(AuditLog auditLog)
    {
        try
        {
            await _auditLogger.LogAsync(auditLog, _logger.Log);
            _logger.Info($"Received AuditLog - CorrelationId: {auditLog.CorrelationId}");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error occurred in AuditLog-ProcessAsync(). Request ID: {auditLog?.CorrelationId}");
        }
    }
}


#endregion

using OF.ProductData.CentralBankReceiverWorker.IServices;
using OF.ProductData.Common.NLog;
using OF.ProductData.Model.EFModel.Audit;

namespace OF.ProductData.CentralBankReceiverWorker.Services;

public class AuditLogger : IAuditLogger
{

    private readonly AuditLogDbContext _context;
    public AuditLogger(AuditLogDbContext context)
    {
        _context = context;
    }
    public async Task LogAsync(AuditLog log, BaseLogger logger)
    {
        string logPrefix = string.Empty;
        logPrefix = $"CorrelationId: {log.CorrelationId} || ";

        try
        {
            _context.auditLogs.Add(log);
            await _context.SaveChangesAsync();
            logger.Info($"{logPrefix}LogAsync is done. Return Id: {log.Id}");
        }
        catch (DbUpdateException dbEx)
        {
            logger.Error(dbEx, $"{logPrefix}Database update error occurred while saving LogAsync.");
            throw;
        }
        catch (Exception ex)
        {
            logger.Error(ex, $"{logPrefix}An error occurred while saving LogAsync.");
            throw;
        }
    }


}

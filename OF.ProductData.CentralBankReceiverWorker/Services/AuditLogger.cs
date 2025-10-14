using OF.ProductData.CentralBankReceiverWorker.IServices;
using OF.ProductData.Model.EFModel.Audit;

namespace OF.ProductData.CentralBankReceiverWorker.Services;

public class AuditLogger : IAuditLogger
{
    
    private readonly AuditLogDbContext _context;
    public AuditLogger(AuditLogDbContext context)
    {
        _context = context;
    }
    public async Task LogAsync(AuditLog log, Logger logger)
    {
        try
        {
            _context.auditLogs.Add(log);
            await _context.SaveChangesAsync();
            logger.Info($"LogAsync is done. CorrelationId: {log.CorrelationId},  Retrun Id:{log.Id})");
        }
        catch (DbUpdateException dbEx)
        {
            logger.Error(dbEx, "Database update error occurred while saving LogAsync.");
            throw; // Rethrow or return a special value depending on your error handling strategy
        }
        catch (Exception ex)
        {
            logger.Error(ex, $"CorrelationId: {log.CorrelationId} || An error occurred while saving LogAsync.");
            throw;
        }
    }

    
}

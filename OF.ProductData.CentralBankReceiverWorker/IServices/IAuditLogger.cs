using OF.ProductData.Model.EFModel.Audit;

namespace OF.ProductData.CentralBankReceiverWorker.IServices;
public interface IAuditLogger
{
    Task LogAsync(AuditLog log, Logger logger);
}
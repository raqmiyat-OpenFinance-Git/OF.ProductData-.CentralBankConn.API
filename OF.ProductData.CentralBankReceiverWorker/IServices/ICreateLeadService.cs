using OF.ProductData.Common.NLog;
using OF.ProductData.Model.EFModel.CreateLead;

namespace OF.ProductData.CentralBankReceiverWorker.IServices;
public interface ICreateLeadService
{
    Task AddCreateLeadAsync(EFCreateLeadRequest leadRequest,    BaseLogger logger);
    Task AddCreateLeadHeaderAsync(EFCreateLeadHeaderRequest leadHeader, long RequestId,BaseLogger logger);
    Task AddCreateLeadResponseAsync(long id,Guid CorrelationId, EFCreateLeadResponse updatedData, BaseLogger logger);
    Task<long> GetPostCreateLeadIdAsync(Guid correlationId, BaseLogger logger);
    Task<bool> UpdateCreateLeadRequestStatusAsync(long id,Guid CorrelationId, BaseLogger logger);

}
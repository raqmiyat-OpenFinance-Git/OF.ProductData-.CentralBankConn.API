using OF.ProductData.Model.EFModel.CreateLead;

namespace OF.ProductData.CentralBankReceiverWorker.IServices;
public interface ICreateLeadService
{
    Task AddCreateLeadAsync(EFCreateLeadRequest leadRequest, Logger logger);
    Task AddCreateLeadHeaderAsync(EFCreateLeadHeaderRequest leadHeader, long RequestId,Logger logger);
    Task AddCreateLeadResponseAsync(long id,Guid CorrelationId, EFCreateLeadResponse updatedData, Logger logger);
    Task<long> GetPostCreateLeadIdAsync(Guid correlationId, Logger logger);
    Task<bool> UpdateCreateLeadRequestStatusAsync(long id,Guid CorrelationId, Logger logger);

}
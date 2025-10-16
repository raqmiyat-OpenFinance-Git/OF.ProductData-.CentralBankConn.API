using OF.ProductData.Model.EFModel.Products;

namespace OF.ProductData.CentralBankReceiverWorker.IServices;
public interface ICreateLeadService
{
    Task AddCreateLeadAsync(EFCreateLeadRequest leadRequest, Logger logger);
    Task AddCreateLeadResponseAsync(long id,Guid CorrelationId, List<EFCreateLeadResponse> updatedData, Logger logger);
    Task<long> GetPostCreateLeadIdAsync(Guid correlationId, Logger logger);
    Task<bool> UpdateCreateLeadRequestStatusAsync(long id,Guid CorrelationId, Logger logger);

}
using OF.ProductData.Common.NLog;
using OF.ProductData.Model.EFModel.Products;

namespace OF.ProductData.CentralBankReceiverWorker.IServices;
public interface IProductDataService
{
    Task AddProductAsync(EFProductRequest productRequest, BaseLogger logger);
    Task AddProductResponseAsync(long id,Guid CorrelationId, List<EFProductResponse> updatedData, BaseLogger logger);
    Task<long> GetPostProductIdAsync(Guid correlationId, BaseLogger logger);
    Task<bool> UpdateProductRequestStatusAsync(long id,Guid CorrelationId, BaseLogger logger);

}
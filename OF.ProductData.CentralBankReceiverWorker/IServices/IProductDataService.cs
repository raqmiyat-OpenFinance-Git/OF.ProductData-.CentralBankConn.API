using OF.ProductData.Model.EFModel.Products;

namespace OF.ProductData.CentralBankReceiverWorker.IServices;
public interface IProductDataService
{
    Task AddProductAsync(EFProductRequest productRequest, Logger logger);
    Task AddProductResponseAsync(long id,Guid CorrelationId, List<EFProductResponse> updatedData, Logger logger);
    Task<long> GetPostProductIdAsync(Guid correlationId, Logger logger);
    Task<bool> UpdateProductRequestStatusAsync(long id,Guid CorrelationId, Logger logger);

}
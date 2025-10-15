using OF.ProductData.Model.EFModel.Products;

namespace OF.ProductData.CentralBankReceiverWorker.IServices;
public interface IProductDataService
{
    Task AddProductAsync(ProductRequest productRequest, Logger logger);
    Task AddProductResponseAsync(long id,Guid CorrelationId, List<ProductResponse> updatedData, Logger logger);
    Task<long> GetPostProductIdAsync(Guid correlationId, Logger logger);

}
using OF.ProductData.CentralBankReceiverWorker.IServices;
using OF.ProductData.Common.NLog;
using OF.ProductData.Model.CentralBank.Products;
using OF.ProductData.Model.Common;
using OF.ServiceInitiation.CentralBankReceiverWorker.Mappers;

namespace OF.ProductData.CentralBankReceiverWorker.Consumer;

#region MassTransitConsumer
[ExcludeFromConfigureEndpoints]
public class CentralBankCreateLeadResponseConsumer : IConsumer<CbProductResponseWrapper>
{
    private readonly ProductLogger _logger;
    private readonly IDbConnection _dbConnection;
    private readonly IOptions<DatabaseConfig> _databaseConfig;
    private readonly IOptions<StoredProcedureConfig> _storedProcedureConfig;
    private readonly IProductDataService _productService;

    public CentralBankCreateLeadResponseConsumer(ProductLogger logger, IDbConnection dbConnection, IOptions<DatabaseConfig> databaseConfig, IOptions<StoredProcedureConfig> storedProcedureConfig, IProductDataService productService)
    {
        _logger = logger;
        _dbConnection = dbConnection;
        _databaseConfig = databaseConfig;
        _storedProcedureConfig = storedProcedureConfig;
        _productService = productService;
    }

    public async Task Consume(ConsumeContext<CbProductResponseWrapper> context)
    {
        try
        {
            if (context?.Message == null)
            {
                _logger.Warn("CentralBankProductResponse message is null.");
                return;
            }
            _logger.Info($"Received CentralBankProductDataResponseConsumer - CorrelationId: {context?.Message?.CorrelationId}");
       
            await UpdateProduct(context?.Message!);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unhandled exception in CentralBankProductDataResponseConsumer.Consume()");
        }
    }

    public async Task UpdateProduct(CbProductResponseWrapper centralBankProductResponseWrapper)
    {

        try
        {
            ArgumentNullException.ThrowIfNull(centralBankProductResponseWrapper);
            var response = centralBankProductResponseWrapper.centralBankProductResponse;
            CbProductDataResponse productModel = new();
            var paymentResponse = CbPostProductMapper.MapCbPostProductResponsetToEF(centralBankProductResponseWrapper);
            long paymentRequestId = await _productService.GetPostProductIdAsync(centralBankProductResponseWrapper.CorrelationId, _logger.Log);
            await Task.Delay(5000);
            await _productService.AddProductResponseAsync(paymentRequestId,centralBankProductResponseWrapper.CorrelationId, paymentResponse, _logger.Log);
            await _productService.UpdateProductRequestStatusAsync(paymentRequestId, centralBankProductResponseWrapper.CorrelationId, _logger.Log);
           
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error occurred in UpdateProduct(). Request ID: {centralBankProductResponseWrapper?.CorrelationId}");
        }
    }
   

   


}


#endregion

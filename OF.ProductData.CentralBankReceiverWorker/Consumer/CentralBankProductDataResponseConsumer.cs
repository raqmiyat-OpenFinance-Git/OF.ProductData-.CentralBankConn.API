using Dapper;
using OF.ProductData.CentralBankReceiverWorker.IServices;
using OF.ProductData.Common.NLog;
using OF.ProductData.Model.CentralBank.Products;
using OF.ProductData.Model.Common;
using OF.ProductData.Model.EFModel.Products;
using OF.ServiceInitiation.CentralBankReceiverWorker.Mappers;

namespace OF.ProductData.CentralBankReceiverWorker.Consumer;

#region MassTransitConsumer
[ExcludeFromConfigureEndpoints]
public class CentralBankProductDataResponseConsumer : IConsumer<CbProductResponseWrapper>
{
    private readonly ProductLogger _logger;
    private readonly IDbConnection _dbConnection;
    private readonly IOptions<DatabaseConfig> _databaseConfig;
    private readonly IOptions<StoredProcedureConfig> _storedProcedureConfig;
    private readonly IProductDataService _productService;

    public CentralBankProductDataResponseConsumer(ProductLogger logger, IDbConnection dbConnection, IOptions<DatabaseConfig> databaseConfig, IOptions<StoredProcedureConfig> storedProcedureConfig, IProductDataService productService)
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
            await Task.Delay(5000);
            await UpdateProduct(context?.Message!);
            await UpdatePostProductRequestStatusAsync(context?.Message!, _logger.Log);
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
            CbProductResponse balanceresponse = new();
            CbProductResponse productModel = new();
            var paymentResponse = CbPostProductMapper.MapCbPostProductResponsetToEF(centralBankProductResponseWrapper);
            long paymentRequestId = await _productService.GetPostProductIdAsync(centralBankProductResponseWrapper.CorrelationId, _logger.Log);
            await _productService.AddProductResponseAsync(paymentRequestId,centralBankProductResponseWrapper.CorrelationId, paymentResponse, _logger.Log);
            await Task.Delay(5000);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error occurred in UpdateProduct(). Request ID: {centralBankProductResponseWrapper?.CorrelationId}");
        }
    }
    public async Task<bool> UpdatePostProductRequestStatusAsync(CbProductResponseWrapper centralBankProductResponseWrapper, Logger logger)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", centralBankProductResponseWrapper.CorrelationId, DbType.Guid);
            parameters.Add("@Status", "PROCESSED", DbType.String);

            logger.Info($"Calling UpdatePostProductRequestStatusAsync with Correlation: Id={centralBankProductResponseWrapper.CorrelationId}, Status={"PROCESSED"}");

            await _dbConnection.ExecuteAsync(
                "OF_UpdateLfiBalanceRequests",
                parameters,
                commandType: CommandType.StoredProcedure,
                commandTimeout: 1200,
                transaction: null);

            logger.Info($"Payment request updated successfully with . : CorrelationId Id={centralBankProductResponseWrapper.CorrelationId}, Status={"PROCESSED"}");
            return true;
        }
        catch (Exception ex)
        {
            logger.Error(ex, $"Error updating payment request  CorrelationId Id={centralBankProductResponseWrapper.CorrelationId}, Status={"PROCESSED"}");
            throw;
        }
    }

   


}


#endregion

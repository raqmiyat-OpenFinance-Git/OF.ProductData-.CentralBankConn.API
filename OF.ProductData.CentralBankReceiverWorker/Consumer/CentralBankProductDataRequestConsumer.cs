using OF.ProductData.CentralBankReceiverWorker.IServices;
using OF.ProductData.Common.NLog;
using OF.ProductData.Model.CentralBank.Products;
using OF.ProductData.Model.Common;
using OF.ProductData.Model.EFModel.Products;

namespace OF.ProductData.CentralBankReceiverWorker.Consumer;

[ExcludeFromConfigureEndpoints]
public class CentralBankProductDataRequestConsumer : IConsumer<CbProductRequest>
{
    private readonly ProductLogger _logger;
    private readonly IDbConnection _dbConnection;
    private readonly IOptions<DatabaseConfig> _databaseConfig;
    private readonly IOptions<StoredProcedureConfig> _storedProcedureConfig;
    private readonly IProductDataService _Service;
    public CentralBankProductDataRequestConsumer(ProductLogger logger, IDbConnection dbConnection, IOptions<DatabaseConfig> databaseConfig, IOptions<StoredProcedureConfig> storedProcedureConfig, IProductDataService Service)
    {
        _logger = logger;
        _dbConnection = dbConnection;
        _databaseConfig = databaseConfig;
        _storedProcedureConfig = storedProcedureConfig;
        _Service = Service;
    }

    public async Task Consume(ConsumeContext<CbProductRequest> context)
    {
        try
        {
            if (context?.Message == null)
            {
                _logger.Warn("CentralBankProductDataRequest message is null.");
                return;
            }
            _logger.Info($"Received CentralBankProductDataRequestConsumer - CorrelationId: {context.Message.CorrelationId!}");

            await CreateProduct(context.Message);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unhandled exception in CentralBankProductDataRequestConsumer.Consume()");
            throw;
        }
    }
    public async Task CreateProduct(CbProductRequest request)
    {
        try
        {

            var productRequest = new ProductRequest
            {
                CorrelationId = request.CorrelationId,
                Authorization = request.Authorization,
                CustomerIpAddress = request.CustomerIpAddress ?? string.Empty,
                PageSize = request.PageSize,
                PageNumber = request.PageNumber,
                ProductCategory = request.ProductCategory ?? string.Empty,
                IsShariaCompliant = request.IsShariaCompliant,
                LastUpdatedDateTime = Convert.ToString(request.LastUpdatedDateTime),
                SortOrder = request.SortOrder ?? string.Empty,
                SortField = request.SortField ?? string.Empty,

                CreatedBy = "System",
                CreatedOn = DateTime.UtcNow,
                Status = "PENDING",
                RequestJson = request.RequestJson ?? string.Empty

            };

            await _Service.AddProductAsync(productRequest, _logger.Log);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error occurred in CreateProduct(). Request ID: {request?.CorrelationId}");
        }
    }
}

using OF.DataSharing.Model.CentralBank.CoPQuery;
using OF.ProductData.CentralBankReceiverWorker.IServices;
using OF.ProductData.Common.NLog;
using OF.ProductData.Model.CentralBank.Products;
using OF.ProductData.Model.Common;
using OF.ProductData.Model.EFModel.Products;
using OF.ServiceInitiation.CentralBankReceiverWorker.Mappers;

namespace OF.ProductData.CentralBankReceiverWorker.Consumer;

[ExcludeFromConfigureEndpoints]
public class CentralBankCreateLeadRequestConsumer : IConsumer<CbPostCreateLeadRequestDto>
{
    private readonly CreateLeadLogger _logger;
    private readonly IDbConnection _dbConnection;
    private readonly IOptions<DatabaseConfig> _databaseConfig;
    private readonly IOptions<StoredProcedureConfig> _storedProcedureConfig;
    private readonly ICreateLeadService _Service;
    public CentralBankCreateLeadRequestConsumer(CreateLeadLogger logger, IDbConnection dbConnection, IOptions<DatabaseConfig> databaseConfig, IOptions<StoredProcedureConfig> storedProcedureConfig, ICreateLeadService Service)
    {
        _logger = logger;
        _dbConnection = dbConnection;
        _databaseConfig = databaseConfig;
        _storedProcedureConfig = storedProcedureConfig;
        _Service = Service;
    }

    public async Task Consume(ConsumeContext<CbPostCreateLeadRequestDto> context)
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
    public async Task CreateProduct(CbPostCreateLeadRequestDto request)
    {
        try
        {

            var (headerEntity, leadEntity) = CbPostCreateLeadMapper.MapCbPostLeadResponseToEF(request);
            await _Service.AddCreateLeadAsync(leadEntity, _logger.Log);
            var requestid = leadEntity.RequestId;
            await _Service.AddCreateLeadHeaderAsync(headerEntity, requestid, _logger.Log);

           _logger.Info($"PaymentRequest inserted. Id = {request.CorrelationId}");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error occurred in CreateProduct(). Request ID: {request?.CorrelationId}");
        }
    }
}

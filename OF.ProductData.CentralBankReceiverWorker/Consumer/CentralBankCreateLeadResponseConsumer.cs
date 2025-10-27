using OF.ProductData.CentralBankReceiverWorker.IServices;
using OF.ProductData.Common.NLog;
using OF.ProductData.Model.CentralBank.Products;
using OF.ProductData.Model.Common;
using OF.ProductData.Model.CoreBank.Products;
using OF.ServiceInitiation.CentralBankReceiverWorker.Mappers;

namespace OF.ProductData.CentralBankReceiverWorker.Consumer;

#region MassTransitConsumer
[ExcludeFromConfigureEndpoints]
public class CentralBankCreateLeadResponseConsumer : IConsumer<CbCreateLeadResponseWrapper>
{
    private readonly ProductLogger _logger;
    private readonly IDbConnection _dbConnection;
    private readonly IOptions<DatabaseConfig> _databaseConfig;
    private readonly IOptions<StoredProcedureConfig> _storedProcedureConfig;
    private readonly ICreateLeadService _leadService;

    public CentralBankCreateLeadResponseConsumer(ProductLogger logger, IDbConnection dbConnection, IOptions<DatabaseConfig> databaseConfig, IOptions<StoredProcedureConfig> storedProcedureConfig, ICreateLeadService leadService)
    {
        _logger = logger;
        _dbConnection = dbConnection;
        _databaseConfig = databaseConfig;
        _storedProcedureConfig = storedProcedureConfig;
        _leadService = leadService;
    }

    public async Task Consume(ConsumeContext<CbCreateLeadResponseWrapper> context)
    {
        try
        {
            if (context?.Message == null)
            {
                _logger.Warn("CentralBankCreateLeadResponse message is null.");
                return;
            }
            _logger.Info($"Received CentralBankCreateLeadResponseConsumer - CorrelationId: {context?.Message?.CorrelationId}");
       
            await UpdateLead(context?.Message!);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unhandled exception in CentralBankCreateLeadResponseConsumer.Consume()");
        }
    }

    public async Task UpdateLead(CbCreateLeadResponseWrapper centralBankLeadResponseWrapper)
    {

        try
        {
            ArgumentNullException.ThrowIfNull(centralBankLeadResponseWrapper);
            var response = centralBankLeadResponseWrapper.centralBankCreateLeadResponse;
            CbsCreateLeadResponse Model = new();
            long RequestId = await _leadService.GetPostCreateLeadIdAsync(centralBankLeadResponseWrapper.CorrelationId, _logger.Log);
            var LeadResponse = CbPostCreateLeadMapper.MapCbPostCreateLeadResponseToEF(centralBankLeadResponseWrapper, RequestId);
            await Task.Delay(5000);
            await _leadService.AddCreateLeadResponseAsync(RequestId, centralBankLeadResponseWrapper.CorrelationId, LeadResponse, _logger.Log);
            await _leadService.UpdateCreateLeadRequestStatusAsync(RequestId, centralBankLeadResponseWrapper.CorrelationId, _logger.Log);
           
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error occurred in UpdateLead(). CorrelationId: {centralBankLeadResponseWrapper?.CorrelationId}");
        }
    }
   

   


}


#endregion

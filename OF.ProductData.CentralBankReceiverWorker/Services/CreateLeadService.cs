using Dapper;
using OF.ProductData.CentralBankReceiverWorker.IServices;
using OF.ProductData.Common.NLog;
using OF.ProductData.Model.EFModel.CreateLead;

namespace OF.ProductData.CentralBankReceiverWorker.Services;

public class CreateLeadService : ICreateLeadService
{

    private readonly EntityDbContext _context;
    private readonly IDbConnection _dbConnection;
    public CreateLeadService(EntityDbContext context, IDbConnection dbConnection)
    {
        _context = context;
        _dbConnection = dbConnection;
    }
    public async Task AddCreateLeadAsync(EFCreateLeadRequest LeadRequest, BaseLogger logger)
    {
        string logPrefix = string.Empty;
        logPrefix = $"RequestId: {LeadRequest.RequestId} || ";
        try
        {
            _context.createLeadRequest!.Add(LeadRequest);
            await _context.SaveChangesAsync();
            logger.Info($"{logPrefix}AddCreateLeadAsync is done. RequestId: {LeadRequest.RequestId}");
        }
        catch (DbUpdateException dbEx)
        {
            logger.Error(dbEx, $"{logPrefix}Database update error occurred while saving AddCreateLeadAsync.");
            throw;
        }
        catch (Exception ex)
        {
            logger.Error(ex, $"{logPrefix}An error occurred while saving AddCreateLeadAsync.");
            throw;
        }
    }

    public async Task AddCreateLeadHeaderAsync(EFCreateLeadHeaderRequest LeadHeader, long requestid, BaseLogger logger)
    {
        string logPrefix = string.Empty;
        logPrefix = $"RequestId: {requestid} || ";
        try
        {
            LeadHeader.RequestId = requestid;
            _context.createLeadHeaderRequest!.Add(LeadHeader);
            await _context.SaveChangesAsync();
            logger.Info($"{logPrefix}AddCreateLeadHeaderAsync is done. RequestId: {LeadHeader.RequestId}");
        }
        catch (DbUpdateException dbEx)
        {
            logger.Error(dbEx, $"{logPrefix}Database update error occurred while saving AddCreateLeadHeaderAsync.");
            throw;
        }
        catch (Exception ex)
        {
            logger.Error(ex, $"{logPrefix}An error occurred while saving AddCreateLeadHeaderAsync.");
            throw;
        }
    }


    public async Task AddCreateLeadResponseAsync(long id, Guid CorrelationId, EFCreateLeadResponse LeadResponse, BaseLogger logger)
    {
        string logPrefix = string.Empty;
        logPrefix = $"CorrelationId: {CorrelationId} || ";
        try
        {
            LeadResponse.RequestId = id;
            await _context.createLeadResponse!.AddRangeAsync(LeadResponse);
            await _context.SaveChangesAsync();
            logger.Info($"{logPrefix}AddCreateLeadResponseAsync is done. RequestId: {LeadResponse.RequestId}");
        }
        catch (DbUpdateException dbEx)
        {
            logger.Error(dbEx, $"{logPrefix}Database update error occurred while saving AddCreateLeadResponseAsync.");
            throw;
        }
        catch (Exception ex)
        {
            logger.Error(ex, $"{logPrefix}RequestId: {LeadResponse!.RequestId} || An error occurred while saving AddCreateLeadResponseAsync.");
            throw;
        }
    }

    public async Task<long> GetPostCreateLeadIdAsync(Guid correlationId, BaseLogger logger)
    {
        string logPrefix = string.Empty;
        logPrefix = $"CorrelationId: {correlationId} || ";
        long result = 0; // Default return value

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("correlationId", correlationId, DbType.Guid);

            var dbResult = await _dbConnection.ExecuteScalarAsync<long?>(
                "OF_GetCreateLeadRequestId",
                parameters,
                commandTimeout: 1200,
                commandType: CommandType.StoredProcedure,
                transaction: null
            );

            result = dbResult ?? 0; // If null, set to 0 or any default you want

            logger.Info($"{logPrefix}GetPostPaymentRequestsIdAsync is done. Result: {result}");
        }
        catch (Exception ex)
        {
            logger.Error(ex, $"{logPrefix}An error occurred while executing GetPostPaymentRequestsIdAsync.");
            throw;
        }

        return result;
    }

    public async Task<bool> UpdateCreateLeadRequestStatusAsync(long id, Guid correlationId, BaseLogger logger)
    {
        string logPrefix = string.Empty;
        logPrefix = $"CorrelationId: {correlationId} || ";
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int64);
            parameters.Add("@Status", "PROCESSED", DbType.String);

            logger.Info($"{logPrefix}Calling OF_UpdateCreateLeadRequests with Transaction: Id={id}, Status={"PROCESSED"}");

            await _dbConnection.ExecuteAsync(
                "OF_UpdateCreateLeadRequests",
                parameters,
                commandType: CommandType.StoredProcedure,
                commandTimeout: 1200,
                transaction: null);

            logger.Info($"{logPrefix}Payment request updated successfully with Transaction. Id={id}, Status={"PROCESSED"}");
            return true;
        }
        catch (Exception ex)
        {
            logger.Error(ex, $"{logPrefix}Error updating payment request. Id={id}, Status={"PROCESSED"}");
            throw;
        }
    }

}

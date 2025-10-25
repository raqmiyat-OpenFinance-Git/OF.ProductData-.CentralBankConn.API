using Dapper;
using OF.ProductData.CentralBankReceiverWorker.IServices;
using OF.ProductData.Model.EFModel.Products;

namespace OF.ProductData.CentralBankReceiverWorker.Services;

public class CreateLeadService : ICreateLeadService
{

    private readonly EntityDbContext _context;
    private readonly IDbConnection _dbConnection;
    public CreateLeadService(EntityDbContext context,IDbConnection dbConnection)
    {
        _context = context;
        _dbConnection = dbConnection;
    }
    public async Task AddCreateLeadAsync(EFCreateLeadRequest LeadRequest, Logger logger)
    {
        try
        {
            _context.createLeadRequest!.Add(LeadRequest);
            await _context.SaveChangesAsync();
            logger.Info($"AddCreateLeadAsync is done. RequestId: {LeadRequest.RequestId}");
        }
        catch (DbUpdateException dbEx)
        {
            logger.Error(dbEx, "Database update error occurred while saving AddCreateLeadAsync.");
            throw; 
        }
        catch (Exception ex)
        {
            logger.Error(ex, $"RequestId: {LeadRequest.RequestId} || An error occurred while saving AddCreateLeadAsync.");
            throw;
        }
    }

    public async Task AddCreateLeadHeaderAsync(EFCreateLeadHeaderRequest LeadHeader, long requestid, Logger logger)
    {
        try
        {
            LeadHeader.RequestId = requestid;
            _context.createLeadHeaderRequest!.Add(LeadHeader);
            await _context.SaveChangesAsync();
            logger.Info($"AddCreateLeadHeaderAsync is done. RequestId: {LeadHeader.RequestId}");
        }
        catch (DbUpdateException dbEx)
        {
            logger.Error(dbEx, "Database update error occurred while saving AddCreateLeadHeaderAsync.");
            throw;
        }
        catch (Exception ex)
        {
            logger.Error(ex, $"RequestId: {LeadHeader.RequestId} || An error occurred while saving AddCreateLeadHeaderAsync.");
            throw;
        }
    }


    public async Task AddCreateLeadResponseAsync(long id,Guid CorrelationId, EFCreateLeadResponse LeadResponse, Logger logger)
    {
        try
        {
            LeadResponse.RequestId = id;
            await _context.createLeadResponse!.AddRangeAsync(LeadResponse);
            await _context.SaveChangesAsync();
           logger.Info($"AddCreateLeadResponseAsync is done. RequestId: {LeadResponse.RequestId}");
        }
        catch (DbUpdateException dbEx)
        {
            logger.Error(dbEx, "Database update error occurred while saving AddCreateLeadResponseAsync.");
            throw; // Rethrow or return a special value depending on your error handling strategy
        }
        catch (Exception ex)
        {
            logger.Error(ex, $"RequestId: {LeadResponse!.RequestId} || An error occurred while saving AddCreateLeadResponseAsync.");
            throw;
        }
    }

    public async Task<long> GetPostCreateLeadIdAsync(Guid correlationId, Logger logger)
    {
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

            logger.Info($"GetPostPaymentRequestsIdAsync is done. CorrelationId: {correlationId}, Result: {result}");
        }
        catch (Exception ex)
        {
            logger.Error(ex, $"CorrelationId: {correlationId} || An error occurred while executing GetPostPaymentRequestsIdAsync.");
            throw;
        }

        return result;
    }

    public async Task<bool> UpdateCreateLeadRequestStatusAsync(long id,Guid correlationId, Logger logger)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int64);
            parameters.Add("@Status", "PROCESSED", DbType.String);

            logger.Info($"Calling OF_UpdateCreateLeadRequests with Transaction: Id={id}, Status={"PROCESSED"}");

            await _dbConnection.ExecuteAsync(
                "OF_UpdateCreateLeadRequests",
                parameters,
                commandType: CommandType.StoredProcedure,
                commandTimeout: 1200,
                transaction: null);

            logger.Info($"Payment request updated successfully with Transaction. CorrelationId: {correlationId}, Id={id}, Status={"PROCESSED"}");
            return true;
        }
        catch (Exception ex)
        {
            logger.Error(ex, $"Error updating payment request. CorrelationId: {correlationId}, Id={id}, Status={"PROCESSED"}");
            throw;
        }
    }

}

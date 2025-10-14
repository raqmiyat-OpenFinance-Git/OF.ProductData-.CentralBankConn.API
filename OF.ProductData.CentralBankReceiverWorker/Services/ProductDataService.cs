using Dapper;
using OF.ProductData.CentralBankReceiverWorker.IServices;
using OF.ProductData.Model.EFModel.Products;
using System.Data.Common;

namespace OF.ProductData.CentralBankReceiverWorker.Services;

public class ProductDataService : IProductDataService
{

    private readonly EntityDbContext _context;
    private readonly IDbConnection _dbConnection;
    public ProductDataService(EntityDbContext context,IDbConnection dbConnection)
    {
        _context = context;
        _dbConnection = dbConnection;
    }
    public async Task AddProductAsync(ProductRequest productRequest, Logger logger)
    {
        try
        {
            _context.ProductRequest!.Add(productRequest);
            await _context.SaveChangesAsync();
            logger.Info($"AddProductAsync is done. RequestId: {productRequest.RequestId}");
        }
        catch (DbUpdateException dbEx)
        {
            logger.Error(dbEx, "Database update error occurred while saving AddProductAsync.");
            throw; 
        }
        catch (Exception ex)
        {
            logger.Error(ex, $"RequestId: {productRequest.RequestId} || An error occurred while saving AddProductAsync.");
            throw;
        }
    }


    public async Task AddProductResponseAsync(long id,Guid CorrelationId, List<ProductResponse> ProductResponse, Logger logger)
    {
        try
        {
            ProductResponse.FirstOrDefault()!.RequestId = id;
            await _context.ProductResponse!.AddRangeAsync(ProductResponse);
            await _context.SaveChangesAsync();
           logger.Info($"AddProductResponseAsync is done. ProductId: {ProductResponse.FirstOrDefault()!.ProductId}");
        }
        catch (DbUpdateException dbEx)
        {
            logger.Error(dbEx, "Database update error occurred while saving AddProductResponseAsync.");
            throw; // Rethrow or return a special value depending on your error handling strategy
        }
        catch (Exception ex)
        {
            logger.Error(ex, $"RequestId: {ProductResponse.FirstOrDefault()!.RequestId} || An error occurred while saving AddProductResponseAsync.");
            throw;
        }
    }

    public async Task<long> GetPostProductIdAsync(Guid correlationId, Logger logger)
    {
        long result = 0; // Default return value

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("correlationId", correlationId, DbType.Guid);

            var dbResult = await _dbConnection.ExecuteScalarAsync<long?>(
                "OF_GetProductDataRequestId",
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
}

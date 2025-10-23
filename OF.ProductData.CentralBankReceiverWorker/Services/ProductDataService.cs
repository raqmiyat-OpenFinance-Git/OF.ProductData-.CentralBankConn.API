using Dapper;
using OF.ProductData.CentralBankReceiverWorker.IServices;
using OF.ProductData.Model.EFModel.Products;

namespace OF.ProductData.CentralBankReceiverWorker.Services;

public class ProductDataService : IProductDataService
{

    private readonly EntityDbContext _context;
    private readonly IDbConnection _dbConnection;
    public ProductDataService(EntityDbContext context, IDbConnection dbConnection)
    {
        _context = context;
        _dbConnection = dbConnection;
    }
    public async Task AddProductAsync(EFProductRequest productRequest, Logger logger)
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


    public async Task AddProductResponseAsync(long id, Guid CorrelationId, List<EFProductResponse> ProductResponse, Logger logger)
    {
        try
        {
           

            var ProductCategory = _context.ProductRequest!.Where(r => r.RequestId == id).Select(s => s.ProductCategory).FirstOrDefault();

            var response = ProductResponse.FirstOrDefault();
            if (response == null) return;

            // Add parent only; EF tracks children
            _context.ProductResponse!.Add(response);
            await _context.SaveChangesAsync();

            if (ProductCategory?.Trim().Equals("CurrentAccount", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                var currentAccount = response.CurrentAccount!.FirstOrDefault();
                if (currentAccount == null) return;

                _context.CurrentAccounts.Add(currentAccount);
                await _context.SaveChangesAsync();
            }
            else if(ProductCategory?.Trim().Equals("SavingsAccount", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                var SavingsAccount = response.SavingsAccount!.FirstOrDefault();
                if (SavingsAccount == null) return;

                _context.SavingsAccounts.Add(SavingsAccount);
                await _context.SaveChangesAsync();

            }
            else if(ProductCategory?.Trim().Equals("CreditCard", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                var CreditCard = response.CreditCard!.FirstOrDefault();
                if (CreditCard == null) return;

                _context.CreditCards.Add(CreditCard);
                await _context.SaveChangesAsync();

            }
            else if (ProductCategory?.Trim().Equals("Loan", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                var PersonalLoan = response.PersonalLoan!.FirstOrDefault();
                if (PersonalLoan == null) return;

                _context.PersonalLoans.Add(PersonalLoan);
                await _context.SaveChangesAsync();

            }   

            else if (ProductCategory?.Trim().Equals("Mortgage", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                var Mortgage = response.Mortgage!.FirstOrDefault();
                if (Mortgage == null) return;

                _context.Mortgages.Add(Mortgage);
                await _context.SaveChangesAsync();

            }


            else if (ProductCategory?.Trim().Equals("ProfitSharingRate", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                var ProfitSharingRate = response.ProfitSharingRate!.FirstOrDefault();
                if (ProfitSharingRate == null) return;


                _context.ProfitSharingRate.Add(ProfitSharingRate);
                await _context.SaveChangesAsync();

            }

            else if (ProductCategory?.Trim().Equals("FinanceProfitRate", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                var FinanceProfitRate = response.FinanceProfitRate!.FirstOrDefault();
                if (FinanceProfitRate == null) return;

                _context.FinanceProfitRate.Add(FinanceProfitRate);
                await _context.SaveChangesAsync();

            }
            else
            {
                var currentAccount = response.CurrentAccount!.FirstOrDefault();
                if (currentAccount == null) return;

                _context.CurrentAccounts.Add(currentAccount);
                await _context.SaveChangesAsync();



                var SavingsAccount = response.SavingsAccount!.FirstOrDefault();
                if (SavingsAccount == null) return;

                _context.SavingsAccounts.Add(SavingsAccount);
                await _context.SaveChangesAsync();


                var CreditCard = response.CreditCard!.FirstOrDefault();
                if (CreditCard == null) return;

                _context.CreditCards.Add(CreditCard);
                await _context.SaveChangesAsync();


                var PersonalLoan = response.PersonalLoan!.FirstOrDefault();
                if (PersonalLoan == null) return;

                _context.PersonalLoans.Add(PersonalLoan);
                await _context.SaveChangesAsync();


                var Mortgage = response.Mortgage!.FirstOrDefault();
                if (Mortgage == null) return;

                _context.Mortgages.Add(Mortgage);
                await _context.SaveChangesAsync();


                var ProfitSharingRate = response.ProfitSharingRate!.FirstOrDefault();
                if (ProfitSharingRate == null) return;


                _context.ProfitSharingRate.Add(ProfitSharingRate);
                await _context.SaveChangesAsync();


                var FinanceProfitRate = response.FinanceProfitRate!.FirstOrDefault();
                if (FinanceProfitRate == null) return;

                _context.FinanceProfitRate.Add(FinanceProfitRate);
                await _context.SaveChangesAsync();

            }

            logger.Info($"AddProductResponseAsync is done. ProductId: {ProductResponse.FirstOrDefault()!.ProductId}");
        }
        catch (DbUpdateException dbEx)
        {
            logger.Error(dbEx, "Database update error occurred while saving AddProductResponseAsync.");
            throw; 
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

    public async Task<bool> UpdateProductRequestStatusAsync(long id, Guid correlationId, Logger logger)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int64);
            parameters.Add("@Status", "PROCESSED", DbType.String);

            logger.Info($"Calling OF_UpdateProductRequests with Transaction: Id={id}, Status={"PROCESSED"}");

            await _dbConnection.ExecuteAsync(
                "OF_UpdateProductRequests",
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

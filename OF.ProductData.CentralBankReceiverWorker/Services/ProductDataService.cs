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

            _context.ProductResponse!.Add(response);
            await _context.SaveChangesAsync();

            if (ProductCategory?.Trim().Equals("CurrentAccount", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                var currentAccount = response.CurrentAccount!.FirstOrDefault();
                var depositRates = response.DepositRates!.FirstOrDefault();
                var RewardsBenefits = response.RewardsBenefits!.FirstOrDefault();

                if (currentAccount == null) return;

                _context.CurrentAccounts.Add(currentAccount);
                await _context.SaveChangesAsync();
                if (depositRates != null)
                {
                    depositRates.RequestId = currentAccount.RequestId; 
                    _context.DepositRates.Add(depositRates);
                    await _context.SaveChangesAsync();
                }
                if (RewardsBenefits != null)
                {
                    RewardsBenefits.RequestId = currentAccount.RequestId;
                    _context.RewardsBenefits.Add(RewardsBenefits);
                    await _context.SaveChangesAsync();
                }
            }
            else if(ProductCategory?.Trim().Equals("SavingsAccount", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                var SavingsAccount = response.SavingsAccount!.FirstOrDefault();
                var depositRates = response.DepositRates!.FirstOrDefault();
                var RewardsBenefits = response.RewardsBenefits!.FirstOrDefault();

                if (SavingsAccount == null) return;

                _context.SavingsAccounts.Add(SavingsAccount);
                await _context.SaveChangesAsync();

                if (depositRates != null)
                {
                    depositRates.RequestId = SavingsAccount.RequestId;
                    _context.DepositRates.Add(depositRates);
                    await _context.SaveChangesAsync();
                }
                if (RewardsBenefits != null)
                {
                    RewardsBenefits.RequestId = SavingsAccount.RequestId;
                    _context.RewardsBenefits.Add(RewardsBenefits);
                    await _context.SaveChangesAsync();
                }
            }
            else if(ProductCategory?.Trim().Equals("CreditCard", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                var CreditCard = response.CreditCard!.FirstOrDefault();
                var FinanceRate = response.FinanceRates!.FirstOrDefault();
                var tenor = response.Tenor!.FirstOrDefault();
                var RewardsBenefits = response.RewardsBenefits!.FirstOrDefault();
                if (CreditCard == null) return;

                _context.CreditCards.Add(CreditCard);
                await _context.SaveChangesAsync();

                if(FinanceRate != null)
                {
                    FinanceRate.RequestId = CreditCard.RequestId;
                    _context.FinanceRates.Add(FinanceRate);
                    await _context.SaveChangesAsync();
                }
                if(tenor != null)
                {
                    tenor.RequestId = CreditCard.RequestId;
                    _context.Tenor.Add(tenor);
                    await _context.SaveChangesAsync();
                }
                if(RewardsBenefits != null)
                {
                    RewardsBenefits.RequestId = CreditCard.RequestId;
                    _context.RewardsBenefits.Add(RewardsBenefits);
                    await _context.SaveChangesAsync();
                }

            }
            else if (ProductCategory?.Trim().Equals("Finance", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                var Finance = response.Finance!.FirstOrDefault();
                var FinanceRate = response.FinanceRates!.FirstOrDefault();
                var tenor = response.Tenor!.FirstOrDefault();
                var AssetBacked = response.AssetBacked!.FirstOrDefault();
                if (Finance == null) return;

                _context.Finance.Add(Finance);
                await _context.SaveChangesAsync();

                if(FinanceRate != null)
                {
                    FinanceRate.RequestId = Finance.RequestId;
                    _context.FinanceRates.Add(FinanceRate);
                    await _context.SaveChangesAsync();
                }
                if(tenor != null)
                {
                    tenor.RequestId = Finance.RequestId;
                    _context.Tenor.Add(tenor);
                    await _context.SaveChangesAsync();
                }
                if (AssetBacked != null)
                {
                    AssetBacked.RequestId = Finance.RequestId;
                    _context.AssetBacked.Add(AssetBacked);
                    await _context.SaveChangesAsync();
                }
            }   

            else if (ProductCategory?.Trim().Equals("Mortgage", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                var Mortgage = response.Mortgage!.FirstOrDefault();
                var FinanceRate = response.FinanceRates!.FirstOrDefault();
                var tenor = response.Tenor!.FirstOrDefault();
                var AssetBacked = response.AssetBacked!.FirstOrDefault();
                if (Mortgage == null) return;

                _context.Mortgages.Add(Mortgage);
                await _context.SaveChangesAsync();

                if (FinanceRate != null)
                {
                    FinanceRate.RequestId = Mortgage.RequestId;
                    _context.FinanceRates.Add(FinanceRate);
                    await _context.SaveChangesAsync();
                }
                if (tenor != null)
                {
                    tenor.RequestId = Mortgage.RequestId;
                    _context.Tenor.Add(tenor);
                    await _context.SaveChangesAsync();
                }
                if (AssetBacked != null)
                {
                    AssetBacked.RequestId = Mortgage.RequestId;
                    _context.AssetBacked.Add(AssetBacked);
                    await _context.SaveChangesAsync();
                }
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


                var Finance = response.Finance!.FirstOrDefault();
                if (Finance == null) return;

                _context.Finance.Add(Finance);
                await _context.SaveChangesAsync();


                var Mortgage = response.Mortgage!.FirstOrDefault();
                if (Mortgage == null) return;

                _context.Mortgages.Add(Mortgage);
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

            result = dbResult ?? 0; 

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

            logger.Info($"Calling OF_UpdateProductRequests with Product: Id={id}, Status={"PROCESSED"}");

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

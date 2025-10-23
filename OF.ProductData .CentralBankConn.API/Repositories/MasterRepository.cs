using OF.ProductData.Model.Common;
using OF.ProductData.Model.EFModel;
using ErrorResponse = OF.ProductData.Model.Common.ErrorResponse;

namespace OF.ProductData.CentralBankConn.API.Repositories;

public class MasterRepository : IMasterRepository
{
    private readonly IDistributedCache _distributedCache;
    private readonly IOptions<RedisCacheSettings> _redisCacheSettings;
    private readonly IDbConnection _dbConnection;

    public MasterRepository(IDistributedCache distributedCache, IOptions<RedisCacheSettings> redisCacheSettings, IDbConnection dbConnection)
    {
        _distributedCache = distributedCache;
        _redisCacheSettings = redisCacheSettings;
        _dbConnection = dbConnection;
    }

    public async Task<bool> IsDuplicateTransactionIdAsync(string requestType, string transactionId, Logger logger)
    {
        try
        {
            return await SetCacheDataAsync(requestType, transactionId, logger);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error occurred in IsTransactionIdDuplicateAsync()");
            return false;
        }
    }

    private async Task<bool> SetCacheDataAsync(string requestType, string transactionId, Logger logger)
    {
        bool isDuplicate = false;

        try
        {
            List<string> transactionIdList = new();
            string cacheKey = string.Empty;

            if (string.Equals(requestType, "CustomerEnquiry", StringComparison.OrdinalIgnoreCase))
            {
                cacheKey = $"{requestType}:{transactionId}";
                transactionIdList = await GetCustomerCacheDataAsync(cacheKey, logger);
            }

            logger.Debug($"Transaction ID list count before checking: {transactionIdList.Count}");

            if (transactionIdList.Contains(transactionId))
            {
                isDuplicate = true;
            }

            if (!isDuplicate)
            {
                transactionIdList.Add(transactionId);
                var cacheBytes = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(transactionIdList));

                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(_redisCacheSettings.Value.CacheExpirationMinutes))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(_redisCacheSettings.Value.CacheExpirationMinutes));

                await _distributedCache.SetAsync(cacheKey, cacheBytes, options);
            }

            logger.Debug($"Transaction ID list count after checking: {transactionIdList.Count}");
            logger.Debug($"Serialized Transaction ID list: {System.Text.Json.JsonSerializer.Serialize(transactionIdList)}");
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error in SetCacheDataAsync()");
        }

        return isDuplicate;
    }

    private async Task<List<string>> GetCustomerCacheDataAsync(string cacheKey, Logger logger)
    {
        try
        {
            var cacheBytes = await _distributedCache.GetAsync(cacheKey);
            if (cacheBytes != null)
            {
                var cacheString = Encoding.UTF8.GetString(cacheBytes);
                return System.Text.Json.JsonSerializer.Deserialize<List<string>>(cacheString) ?? new List<string>();
            }
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error in GetCustomerCacheDataAsync()");
        }

        return new List<string>();
    }

    public async Task<ErrorResponse> GetErrorResponseAsync(string code, Logger logger)
    {
        var errorResponse = new ErrorResponse();

        try
        {
            logger.Info("GetErrorResponseAsync Invoked");

            var masterTableList = await GetCachedMasterAsync(logger);
            if (masterTableList?.ofCbsMappingCodes == null)
            {
                logger.Warn("CbsMappingCodes list is null.");
                errorResponse.errorCode = "E001";
                errorResponse.errorMessage = "Mapping data not available.";
                return errorResponse;
            }

            var ofCbsMappingCode = masterTableList.ofCbsMappingCodes.Find(a => a.CbsMappingCode == code);
            if (ofCbsMappingCode == null)
            {
                logger.Warn($"Mapping code '{code}' not found.");
                errorResponse.errorCode = "E002";
                errorResponse.errorMessage = "Invalid error code.";
            }
            else
            {
                errorResponse.errorCode = ofCbsMappingCode.CBStatusCode;
                errorResponse.errorMessage = ofCbsMappingCode.Description;
            }

            logger.Info("GetErrorResponseAsync End");
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error occurred in GetErrorResponseAsync()");
        }

        return errorResponse;
    }

    private async Task<MasterTableList> GetCachedMasterAsync(Logger logger)
    {
        var cacheKey = "OfCbsMappingCodeList";
        return await GetCachedDataAsync<MasterTableList>(cacheKey, async () => await GetMasterAsync(logger));
    }

    private async Task<MasterTableList> GetMasterAsync(Logger logger)
    {
        var masterTableList = new MasterTableList();

        try
        {
            var parameters = new DynamicParameters();

            using var reader = await _dbConnection.QueryMultipleAsync(
                "OF_sp_get_CbsMappingCodes",
                parameters,
                commandType: CommandType.StoredProcedure);

            masterTableList.ofCbsMappingCodes = reader.Read<OfCbsMappingCode>().ToList();
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error occurred in GetMasterAsync()");
        }

        return masterTableList;
    }

    private async Task<T> GetCachedDataAsync<T>(string cacheKey, Func<Task<T>> fetchData)
    {
        var cachedData = await _distributedCache.GetAsync(cacheKey);
        if (cachedData != null)
        {
            var cachedDataString = Encoding.UTF8.GetString(cachedData);
            return System.Text.Json.JsonSerializer.Deserialize<T>(cachedDataString)!;
        }

        var data = await fetchData();

        var dataToCache = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(data));
        var options = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(DateTimeOffset.Now.AddMinutes(_redisCacheSettings.Value.CacheExpirationMinutes));

        await _distributedCache.SetAsync(cacheKey, dataToCache, options);

        return data;
    }


}

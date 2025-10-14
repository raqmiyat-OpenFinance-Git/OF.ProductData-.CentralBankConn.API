using ErrorResponse = OF.ProductData.Model.Common.ErrorResponse;

namespace OF.ProductData.CentralBankConn.API.Repositories;
public interface IMasterRepository
{
    Task<bool> IsDuplicateTransactionIdAsync(string requestType, string transactionId, Logger logger);
    Task<ErrorResponse> GetErrorResponseAsync(string code, Logger logger);
}

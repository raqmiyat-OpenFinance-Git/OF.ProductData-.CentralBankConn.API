using NLog;
using OF.ProductData.Model.CoreBank;
using OF.ProductData.Model.CoreBank.Products;
using System.Net;

namespace OF.ProductData.CoreBankConn.API.IServices;
public interface IProductDataService
{
    Task<ApiResult<CbsProductResponse>> PostOnlineEnquiryAsync(CbsProductRequest coreBankProductRequest, Logger logger);
    Task<HttpStatusCode> WarmUp(Logger logger);
}

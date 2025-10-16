using NLog;
using OF.ProductData.Model.CoreBank;
using OF.ProductData.Model.CoreBank.Products;
using System.Net;

namespace OF.ProductData.CoreBankConn.API.IServices;
public interface ICreateLeadService
{
    Task<ApiResult<CbsCreateLeadResponse>> PostOnlineEnquiryAsync(CbsCreateLeadRequest coreBankProductRequest, Logger logger);
    Task<HttpStatusCode> WarmUp(Logger logger);
}

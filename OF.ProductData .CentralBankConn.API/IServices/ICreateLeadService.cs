using OF.ProductData.Model.CentralBank.Products;
using OF.ProductData.Model.CoreBank;
using OF.ProductData.Model.CoreBank.Products;

namespace OF.ProductData.CentralBankConn.API.IServices;

public interface ICreateLeadService
{
    Task<ApiResult<CbsCreateLeadResponse>> GetProductFromCoreBankAsync(CbsCreateLeadRequest cbProductRequest);
    CbProductDataResponse GetCentralBankProductByIdResponse(CbsCreateLeadResponse cbProductResponse, Logger logger);
}

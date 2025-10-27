using OF.ProductData.Model.CentralBank.Products;
using OF.ProductData.Model.CoreBank;
using OF.ProductData.Model.CoreBank.Products;

namespace OF.ProductData.CentralBankConn.API.IServices;

public interface IProductDataService
{
    Task<ApiResult<CbsProductResponse>> GetProductFromCoreBankAsync(CbsProductRequest cbProductRequest);
    CbProductDataResponse GetCentralBankProductByIdResponse(CbsProductResponse cbProductResponse, Logger logger);
    CbProductDataResponse ResponseProductDetails(CbsProductResponse cbProductResponse, string productCategory, Logger logger);
}

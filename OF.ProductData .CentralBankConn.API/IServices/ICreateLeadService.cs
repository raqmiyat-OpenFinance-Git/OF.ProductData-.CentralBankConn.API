using OF.ProductData.Model.CentralBank.CreateLead;
using OF.ProductData.Model.CentralBank.Products;
using OF.ProductData.Model.CoreBank;
using OF.ProductData.Model.CoreBank.Products;

namespace OF.ProductData.CentralBankConn.API.IServices;

public interface ICreateLeadService
{
    Task<ApiResult<CbsCreateLeadResponse>> PostCreateLeadFromCoreBankAsync(CbsCreateLeadRequest cbProductRequest);
    CbPostCreateLeadResponse GetCentralBankCreateLeadResponse(CbsCreateLeadResponse cbProductResponse, Logger logger);
    CbPostCreateLeadResponse GetCreateLeadResponse(CbsCreateLeadResponse cbProductResponse, CbPostCreateLeadRequest leadRequest, Logger logger);
}

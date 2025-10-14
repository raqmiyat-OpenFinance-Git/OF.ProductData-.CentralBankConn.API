using NLog;
using OF.ProductData.Model.EFModel;

namespace OF.ProductData.CoreBankConn.API.Repositories;
public interface ICoreBankRepository
{
    Task<string> GetNextSequenceNoAsync(string serviceName, string referenceNbr, Logger logger);
    Task<long> SaveCoreBankEnquiryAsync(CoreBankEnquiry enquiry, Logger logger);
}

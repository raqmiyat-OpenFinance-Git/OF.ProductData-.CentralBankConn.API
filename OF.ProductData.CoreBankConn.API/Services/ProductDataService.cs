using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;
using OF.ProductData.Common.AES;
using OF.ProductData.Common.Custom;
using OF.ProductData.Common.Helpers;
using OF.ProductData.Common.NLog;
using OF.ProductData.CoreBankConn.API.EFModel;
using OF.ProductData.CoreBankConn.API.IServices;
using OF.ProductData.CoreBankConn.API.Repositories;
using OF.ProductData.Model.Common;
using OF.ProductData.Model.CoreBank;
using OF.ProductData.Model.CoreBank.Products;
using OF.ProductData.Model.EFModel;
using System.Net;
using SecurityParameters = OF.ProductData.Common.AES.SecurityParameters;

namespace OF.ProductData.CoreBankConn.API.Services;

public class ProductDataService : IProductDataService
{
    private readonly SecurityParameters _securityParameters;
    private readonly AesCbcGenericService _aesCbcGenericService;
    private readonly HttpClient _httpClient;
    private readonly IOptions<CoreBankApis> _coreBankApis;
    private readonly ICoreBankRepository _repository;
    public ProductDataService(AesCbcGenericService aesCbcGenericService, IOptions<SecurityParameters> securityParameters, HttpClient httpClient, IOptions<CoreBankApis> coreBankApis, CbsDbContext context, ICoreBankRepository repository)
    {
        _aesCbcGenericService = aesCbcGenericService;
        _httpClient = httpClient;
        _securityParameters = securityParameters.Value;
        _coreBankApis = coreBankApis;
        _repository = repository;
    }

    public async Task<ApiResult<CbsProductResponse>> PostOnlineEnquiryAsync(CbsProductRequest request, BaseLogger logger)
    {
        bool isSuccessful = false;
        CbsProductResponse response = new();
        CoreBankEnquiry enquiry = new();
        PostStatus postStatus = PostStatus.SUCCESS;
        string requestPayload = string.Empty;
        string responsePayload = string.Empty;
        DateTime messageSentAt = DateTime.Now;
        DateTime messageReceivedAt;
        string logPrefix = string.Empty;
        logPrefix = $"CorrelationId: {request.CorrelationId} || ";
        try
        {
            logger.Info($"{logPrefix}PostOnlineEnquiryAsync Initiated.");
            requestPayload = JsonConvert.SerializeObject(request, Formatting.Indented);

            var content = GetStringContent(requestPayload, request.ExternalRefNbr!, logger);

            var url = UrlHelper.CombineUrl(_coreBankApis.Value.BaseUrl!, _coreBankApis.Value.ProductServiceUrl!.GetProductUrl!);

            messageSentAt = DateTime.Now;

            HttpResponseMessage apiResponse = await _httpClient.PostAsync(url, content);

            messageReceivedAt = DateTime.Now;

            responsePayload = await apiResponse.Content.ReadAsStringAsync();

            logger.Info($"{logPrefix}CoreBank API response received. StatusCode: {(int)apiResponse.StatusCode} ({apiResponse.StatusCode})");

            if (apiResponse.IsSuccessStatusCode)
            {
                string message = GetResponseString(responsePayload, request.ExternalRefNbr!, logger);
                response = JsonConvert.DeserializeObject<CbsProductResponse>(message)!;
                if (response == null)
                {

                    logger.Error($"{logPrefix}Failed to deserialize ProductData response.");
                    postStatus = PostStatus.ERROR;
                    enquiry = CreateCoreBankEnquiry(request.CorrelationId, request.ExternalRefNbr!, Utils.GetStatus(postStatus), "500", "Failed to process response.", messageSentAt, messageReceivedAt, requestPayload, responsePayload, logger);
                }
                else
                {
                    logger.Info($"{logPrefix}ProductData response received successfully.");
                    isSuccessful = true;
                    postStatus = PostStatus.SUCCESS;
                    enquiry = CreateCoreBankEnquiry(request.CorrelationId, request.ExternalRefNbr!, Utils.GetStatus(postStatus), "000", "Successful", messageSentAt, messageReceivedAt, requestPayload, responsePayload, logger);
                }
            }
            else
            {
                string statusCode = ((int)apiResponse.StatusCode).ToString() ?? "500";
                postStatus = PostStatus.ERROR;
                enquiry = CreateCoreBankEnquiry(request.CorrelationId, request.ExternalRefNbr!, Utils.GetStatus(postStatus), statusCode, "API Response: FAILED", messageSentAt, messageReceivedAt, requestPayload, responsePayload, logger);

                logger.Error($"{logPrefix}API Response: FAILED. StatusCode: {apiResponse.StatusCode}");
            }


        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException || ex.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase))
        {
            messageReceivedAt = DateTime.Now;
            postStatus = PostStatus.TIMEOUT;
            enquiry = CreateCoreBankEnquiry(request.CorrelationId, request.ExternalRefNbr!, Utils.GetStatus(postStatus), "504", "Request is Timeout", messageSentAt, messageReceivedAt, requestPayload, ex.Message, logger);

            logger.Error(ex, $"{logPrefix}TimeoutException: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            messageReceivedAt = DateTime.Now;
            postStatus = PostStatus.HOST_ERROR;
            enquiry = CreateCoreBankEnquiry(request.CorrelationId, request.ExternalRefNbr!, Utils.GetStatus(postStatus), "502", "Connection failed: " + ex.Message, messageSentAt, messageReceivedAt, requestPayload, ex.Message, logger);

            logger.Error(ex, $"{logPrefix}Network error or server unreachable.");

        }
        catch (Exception ex)
        {
            messageReceivedAt = DateTime.Now;
            postStatus = PostStatus.ERROR;
            enquiry = CreateCoreBankEnquiry(request.CorrelationId, request.ExternalRefNbr!, Utils.GetStatus(PostStatus.ERROR), "500", "n unexpected error occurred. Contact support. " + ex.Message, messageSentAt, messageReceivedAt, requestPayload, ex.Message, logger);

            logger.Error(ex, $"{logPrefix}Exception: {ex.Message}");
        }
        enquiry.Id = await _repository.SaveCoreBankEnquiryAsync(enquiry, logger.Log);

        logger.Info($"{logPrefix}{WirteLog(logger.Log, enquiry)}");
        if (isSuccessful)
        {
            return ApiResultFactory.Success(data: response!, enquiry.ResponsePayload!, "200");

        }
        else
        {
            return ApiResultFactory.Failure<CbsProductResponse>(enquiry.ReturnDescription!, enquiry.ReturnCode!);
        }
    }
    private string WirteLog(Logger logger, CoreBankEnquiry enquiry)
    {
        return $@"
                    ====== CoreBankEnquiry Log ======
                    EnquiryId        : {enquiry.Id}
                    RequestType        : {enquiry.RequestType}
                    ExternalRefNbr     : {enquiry.ExternalReferenceNumber}
                    Status             : {enquiry.Status}
                    Account            : {enquiry.AccountNumber}
                    DebitCredit        : {enquiry.TransactionType}
                    Amount             : {enquiry.Amount}
                    CreatedOn          : {enquiry.CreatedOn:yyyy-MM-dd HH:mm:ss}
                    CreatedBy          : {enquiry.CreatedBy}
                    Module             : {enquiry.Module}
                    ReturnCode         : {enquiry.ReturnCode}
                    ReturnDescription  : {enquiry.ReturnDescription}
                    HostRefNbr         : {enquiry.HostReferenceNumber}
                    SentDatetime       : {enquiry.MessageSentAt:yyyy-MM-dd HH:mm:ss}
                    ReceviedDatetime   : {enquiry.MessageReceivedAt:yyyy-MM-dd HH:mm:ss}
                    RetryCount         : {enquiry.RetryCount}
                    RetryOn            : {enquiry.RetryOn:yyyy-MM-dd HH:mm:ss}
                    Comments           : {enquiry.Comments}
                    Request            : {PciDssSecurity.MaskCardInDynamicJson(enquiry.RequestPayload!, logger)}
                    Response           : {PciDssSecurity.MaskCardInDynamicJson(enquiry.ResponsePayload!, logger)}
                    ===============================";
    }

    public async Task<HttpStatusCode> WarmUp(BaseLogger logger)
    {
        string logPrefix = string.Empty;
        try
        {
            logger.Trace($"{logPrefix}Starting WarmUp() method.");

            HttpResponseMessage apiResponse = await _httpClient.GetAsync(_coreBankApis.Value.BaseUrl);
            logger.Trace($"{logPrefix}API response received with status code: {apiResponse.StatusCode}");

            return apiResponse.StatusCode;
        }
        catch (Exception ex)
        {
            logger.Error(ex, $"{logPrefix}An error occurred in WarmUp(). Exception details: {ex}");
            return HttpStatusCode.InternalServerError;
        }
    }

    private StringContent GetStringContent(string jsonContent, string externalRefNbr, BaseLogger logger)
    {
        string logPrefix = string.Empty;
        try
        {
            if (_securityParameters.IsEncrypted)
            {
                jsonContent = _aesCbcGenericService.Encrypt(jsonContent, _securityParameters.KeyValue!, _securityParameters.InitVector!, logger.Log);
                logger.Debug($"{logPrefix}ExternalRefNbr: {externalRefNbr} || Request payload encrypted.");
            }
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            return content;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error during encryption/decryption", ex);
        }
    }

    private string GetResponseString(string jsonResponse, string externalRefNbr, BaseLogger logger)
    {
        string logPrefix = string.Empty;
        try
        {
            if (_securityParameters.IsEncrypted)
            {
                jsonResponse = _aesCbcGenericService.Decrypt(jsonResponse, _securityParameters.KeyValue!, _securityParameters.InitVector!, logger.Log);
            }
            logger.Debug($"{logPrefix}ExternalRefNbr: {externalRefNbr} || Core response received.");

            return jsonResponse;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error during encryption/decryption", ex);
        }
    }

    private CoreBankEnquiry CreateCoreBankEnquiry(Guid correlationId, string externalRefNbr, string status, string returnCode, string returnDescription, DateTime messageSentAt, DateTime messageReceivedAt, string requestPayload, string responsePayload, BaseLogger logger)
    {
        CoreBankEnquiry coreBankEnquiry = new();
        string logPrefix = string.Empty;
        logPrefix = $"CorrelationId: {correlationId} || ";
        try
        {

            coreBankEnquiry.RequestType = MessageTypeMappings.ProductEnquiry;
            coreBankEnquiry.ExternalReferenceNumber = externalRefNbr;
            coreBankEnquiry.Status = status;
            coreBankEnquiry.AccountNumber = "";
            coreBankEnquiry.TransactionType = "CR";
            coreBankEnquiry.Amount = 0;
            coreBankEnquiry.CreatedOn = DateTime.Now;
            coreBankEnquiry.CreatedBy = "CoreBankAPI";
            coreBankEnquiry.Module = "Inward";
            coreBankEnquiry.ReturnCode = returnCode;
            coreBankEnquiry.ReturnDescription = returnDescription;
            coreBankEnquiry.MessageSentAt = messageSentAt;
            coreBankEnquiry.MessageReceivedAt = messageReceivedAt;
            coreBankEnquiry.RetryCount = 0;
            coreBankEnquiry.RetryOn = null;
            coreBankEnquiry.RequestPayload = requestPayload;
            coreBankEnquiry.ResponsePayload = responsePayload;
            coreBankEnquiry.Comments = "Initiated via CoreBankAPI";
            coreBankEnquiry.LastUpdatedOn = null;

        }
        catch (Exception ex)
        {
            logger.Error(ex, $"{logPrefix}[CreateCoreBankEnquiry] Error: {ex.Message}");
        }
        return coreBankEnquiry;
    }
}

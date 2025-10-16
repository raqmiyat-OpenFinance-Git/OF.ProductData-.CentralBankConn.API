using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;
using OF.ProductData.Common.AES;
using OF.ProductData.Common.Custom;
using OF.ProductData.Common.Helpers;
using OF.ProductData.CoreBankConn.API.EFModel;
using OF.ProductData.CoreBankConn.API.IServices;
using OF.ProductData.CoreBankConn.API.Repositories;
using OF.ProductData.Model.CentralBank.Products;
using OF.ProductData.Model.Common;
using OF.ProductData.Model.CoreBank;
using OF.ProductData.Model.CoreBank.Products;
using OF.ProductData.Model.EFModel;
using System.Net;
using SecurityParameters = OF.ProductData.Common.AES.SecurityParameters;

namespace OF.ProductData.CoreBankConn.API.Services;

public class CreateLeadService : ICreateLeadService
{
    private readonly SecurityParameters _securityParameters;
    private readonly AesCbcGenericService _aesCbcGenericService;
    private readonly HttpClient _httpClient;
    private readonly IOptions<CoreBankApis> _coreBankApis;
    private readonly ICoreBankRepository _repository;
    public CreateLeadService(AesCbcGenericService aesCbcGenericService, IOptions<SecurityParameters> securityParameters, HttpClient httpClient, IOptions<CoreBankApis> coreBankApis, CbsDbContext context, ICoreBankRepository repository)
    {
        _aesCbcGenericService = aesCbcGenericService;
        _httpClient = httpClient;
        _securityParameters = securityParameters.Value;
        _coreBankApis = coreBankApis;
        _repository = repository;
    }

    public async Task<ApiResult<CbsCreateLeadResponse>> PostOnlineEnquiryAsync(CbsCreateLeadRequest request, Logger logger)
    {
        bool isSuccessful = false;
        CbsCreateLeadResponse response = new();
        CoreBankEnquiry enquiry = new();
        PostStatus postStatus = PostStatus.SUCCESS;
        string requestPayload = string.Empty;
        string responsePayload = string.Empty;
        DateTime messageSentAt = DateTime.Now;
        DateTime messageReceivedAt;
        try
        {
            requestPayload = JsonConvert.SerializeObject(request, Formatting.Indented);

            var content = GetStringContent(requestPayload, request.ExternalRefNbr!, logger);

            var url = UrlHelper.CombineUrl(_coreBankApis.Value.BaseUrl!, _coreBankApis.Value.ProductServiceUrl!.GetProductUrl!);

            messageSentAt = DateTime.Now;

            HttpResponseMessage apiResponse = await _httpClient.PostAsync(url, content);

            messageReceivedAt = DateTime.Now;

            responsePayload = await apiResponse.Content.ReadAsStringAsync();

            logger.Debug($"CorrelationId: {request.CorrelationId}, IsSuccessStatusCode:{apiResponse.IsSuccessStatusCode}");

            if (apiResponse.IsSuccessStatusCode)
            {
                string message = GetResponseString(responsePayload, request.ExternalRefNbr!, logger);
                response = JsonConvert.DeserializeObject<CbsCreateLeadResponse>(message)!;
                if (response == null)
                {

                    logger.Error($"CorrelationId: {request.CorrelationId} || Failed to deserialize balance response.");
                    postStatus = PostStatus.ERROR;
                    enquiry = CreateCoreBankEnquiry(request.CorrelationId, request.ExternalRefNbr!, Utils.GetStatus(postStatus), "500", "Failed to process response.", messageSentAt, messageReceivedAt, requestPayload, responsePayload, logger);
                }
                else
                {
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

                logger.Debug($"CorrelationId: {request.CorrelationId} || API Response: FAILED");
            }


        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException || ex.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase))
        {
            messageReceivedAt = DateTime.Now;
            postStatus = PostStatus.TIMEOUT;
            enquiry = CreateCoreBankEnquiry(request.CorrelationId, request.ExternalRefNbr!, Utils.GetStatus(postStatus), "504", "Request is Timeout", messageSentAt, messageReceivedAt, requestPayload, ex.Message, logger);

            logger.Error(ex, $"CorrelationId: {request.CorrelationId!} || TimeoutException: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            messageReceivedAt = DateTime.Now;
            postStatus = PostStatus.HOST_ERROR;
            enquiry = CreateCoreBankEnquiry(request.CorrelationId, request.ExternalRefNbr!, Utils.GetStatus(postStatus), "502", "Connection failed: " + ex.Message, messageSentAt, messageReceivedAt, requestPayload, ex.Message, logger);

            logger.Error(ex, $"CorrelationId: {request.CorrelationId!} || Network error or server unreachable.");

        }
        catch (Exception ex)
        {
            messageReceivedAt = DateTime.Now;
            postStatus = PostStatus.ERROR;
            enquiry = CreateCoreBankEnquiry(request.CorrelationId, request.ExternalRefNbr!, Utils.GetStatus(PostStatus.ERROR), "500", "n unexpected error occurred. Contact support. " + ex.Message, messageSentAt, messageReceivedAt, requestPayload, ex.Message, logger);

            logger.Error(ex, $"CorrelationId: {request.CorrelationId!} || Exception: {ex.Message}");
        }
        enquiry.Id = await _repository.SaveCoreBankEnquiryAsync(enquiry, logger);

        logger.Info(WirteLog(logger, enquiry));
        if (isSuccessful)
        {
            return ApiResultFactory.Success(data: response!, enquiry.ResponsePayload!, "200");

        }
        else
        {
            return ApiResultFactory.Failure<CbsCreateLeadResponse>(enquiry.ReturnDescription!, enquiry.ReturnCode!);
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

    public async Task<HttpStatusCode> WarmUp(Logger logger)
    {
        try
        {
            logger.Trace("Starting WarmUp() method.");

            HttpResponseMessage apiResponse = await _httpClient.GetAsync(_coreBankApis.Value.BaseUrl);
            logger.Trace($"API response received with status code: {apiResponse.StatusCode}");

            return apiResponse.StatusCode;
        }
        catch (Exception ex)
        {
            logger.Error(ex, $"An error occurred in WarmUp(). Exception details: {ex}");
            return HttpStatusCode.InternalServerError;
        }
    }

    private StringContent GetStringContent(string jsonContent, string externalRefNbr, Logger logger)
    {
        try
        {
            if (_securityParameters.IsEncrypted)
            {
                jsonContent = _aesCbcGenericService.Encrypt(jsonContent, _securityParameters.KeyValue!, _securityParameters.InitVector!, logger);
                logger.Debug($"ExternalRefNbr: {externalRefNbr} || Encrypted String: {jsonContent}");
            }
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            return content;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error during encryption/decryption", ex);
        }
    }

    private string GetResponseString(string jsonResponse, string externalRefNbr, Logger logger)
    {
        try
        {
            if (_securityParameters.IsEncrypted)
            {
                jsonResponse = _aesCbcGenericService.Decrypt(jsonResponse, _securityParameters.KeyValue!, _securityParameters.InitVector!, logger);
            }
            logger.Debug($" ExternalRefNbr: {externalRefNbr} || Core Response data: {jsonResponse}");

            return jsonResponse;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error during encryption/decryption", ex);
        }
    }

    private CoreBankEnquiry CreateCoreBankEnquiry(Guid correlationId, string externalRefNbr, string status, string returnCode, string returnDescription, DateTime messageSentAt, DateTime messageReceivedAt, string requestPayload, string responsePayload, Logger logger)
    {
        CoreBankEnquiry coreBankEnquiry = new();
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
            logger.Error(ex, $"[CreateCoreBankEnquiry] CorrelationId: {correlationId} || Error: {ex.Message}");
        }
        return coreBankEnquiry;
    }
}

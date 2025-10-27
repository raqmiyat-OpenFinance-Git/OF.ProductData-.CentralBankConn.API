using OF.DataSharing.Model.CentralBank.CoPQuery;
using OF.ProductData.CentralBankConn.API.IServices;
using OF.ProductData.CentralBankConn.API.Models;
using OF.ProductData.Common.Helpers;
using OF.ProductData.Common.NLog;
using OF.ProductData.Model.CentralBank.CreateLead;
using OF.ProductData.Model.CentralBank.Products;
using OF.ProductData.Model.Common;
using OF.ProductData.Model.CoreBank.Products;
using ErrorResponse = OF.ProductData.Model.Common.ErrorResponse;

namespace OF.ProductData.CentralBankConn.API.Controllers;

public class CreateLeadController : ControllerBase
{
    private readonly ICreateLeadService _service;
    private readonly CreateLeadLogger _logger;
    private readonly SendPointInitialize _sendPointInitialize;
    private readonly IOptions<CoreBankApis> _coreBankApis;
    private readonly IValidator<CbPostCreateLeadHeader> _validator;
    public CreateLeadController(ICreateLeadService service, CreateLeadLogger logger, SendPointInitialize sendPointInitialize, IOptions<CoreBankApis> coreBankApis, IValidator<CbPostCreateLeadHeader> validator)
    {
        _service = service;
        _logger = logger;
        _sendPointInitialize = sendPointInitialize;
        _coreBankApis = coreBankApis;
        _validator = validator;
    }

    [HttpPost]
    [Route("/leads")]
    [ProducesResponseType(typeof(CbPostCreateLeadResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<IActionResult> CreateLead(
              // ==== Required Headers ====
              [Required][FromHeader(Name = "o3-provider-id")] string o3ProviderId,
              [Required][FromHeader(Name = "o3-caller-org-id")] string o3CallerOrgId,
              [Required][FromHeader(Name = "o3-caller-client-id")] string o3CallerClientId,
              [Required][FromHeader(Name = "o3-caller-software-statement-id")] string o3CallerSoftwareStatementId,
              [Required][FromHeader(Name = "o3-api-uri")] string o3ApiUri,
              [Required][FromHeader(Name = "o3-api-operation")] string o3ApiOperation,
              [FromHeader(Name = "o3-caller-interaction-id")] string? o3CallerInteractionId,
              [Required][FromHeader(Name = "o3-ozone-interaction-id")] string o3OzoneInteractionId,
              [Required][FromHeader(Name = "x-fapi-customer-ip-address")] string xFapiCustomerIpAddress,

              // ==== Request Body ====
              [FromBody] CbPostCreateLeadRequest requestBody)
    {
        var stopwatch = Stopwatch.StartNew();

        // Trim headers
        o3ProviderId = o3ProviderId.Trim();
        o3CallerOrgId = o3CallerOrgId.Trim();
        o3CallerClientId = o3CallerClientId.Trim();
        o3CallerSoftwareStatementId = o3CallerSoftwareStatementId.Trim();
        o3ApiUri = o3ApiUri.Trim();
        o3ApiOperation = o3ApiOperation.Trim();
        o3CallerInteractionId = o3CallerInteractionId?.Trim();
        o3OzoneInteractionId = o3OzoneInteractionId.Trim();
        xFapiCustomerIpAddress = xFapiCustomerIpAddress.Trim();

        if (requestBody?.Data == null)
            return BadRequest(new ErrorResponse { errorMessage = "Request body cannot be null." });


        string? correlationId = string.Empty;
        string requestJson = string.Empty;
        string responseJson = string.Empty;
        string endPointUrl = string.Empty;
        Guid guid = Guid.Empty;

        if (!ModelState.IsValid)
        {
            var errors = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            _logger.Error($"Request validation failed: {errors}");
            return BadRequest(new ErrorResponse { errorCode = "401", errorMessage = $"Invalid request: {errors}" });
        }

        try
        {
            _logger.Info("RetrieveProduct invoked.");

            _logger.Info("------------------------------------------------------------------------");


            endPointUrl = UrlHelper.CombineUrl(_coreBankApis.Value.BaseUrl!, _coreBankApis.Value.LeadServiceUrl!.GetLeadUrl!);
            correlationId = HttpContext.Items["X-Correlation-ID"]?.ToString();
            if (!Guid.TryParse(correlationId, out guid))
            {
                _logger.Info($"Invalid CorrelationId: {correlationId}");
                return BadRequest(new ErrorResponse { errorCode = "400", errorMessage = $"Invalid CorrelationId: {correlationId}" });
            }


            CbPostCreateLeadRequestDto cbRequestDto = new CbPostCreateLeadRequestDto
            {
                CorrelationId = guid,

                cbPostCreateLeadHeader = MapHeadersToCentralRequest(
                       o3ProviderId, o3CallerOrgId, o3CallerClientId, o3CallerSoftwareStatementId,
                       o3ApiUri, o3ApiOperation, o3CallerInteractionId,
                       o3OzoneInteractionId, xFapiCustomerIpAddress, guid),

                cbPostCreateLeadRequest = requestBody
            };

            requestJson = JsonConvert.SerializeObject(cbRequestDto, Formatting.Indented);

            _logger.Info($"Request received:\n{requestJson}");

            await _sendPointInitialize.PostLeadRequest!.Send(cbRequestDto);

            _logger.Info($"Request send:\n{requestJson}");

            CbsCreateLeadRequest coreRequest = new()
            {
                CorrelationId = guid
            };

            // call internal API

            _logger.Info($"CoreBank Request Received: {JsonConvert.SerializeObject(coreRequest)}");
            var apiResult = await _service.PostCreateLeadFromCoreBankAsync(coreRequest);

            _logger.Info($"CoreBank Request send: {JsonConvert.SerializeObject(coreRequest)}");


            if (apiResult == null)
            {
                return NotFound(new ErrorResponse { errorCode = "404", errorMessage = "Product data not found." });
            }
            CbCreateLeadResponseWrapper centralBankLeadResponseWrapper = new();
            CbPostCreateLeadResponse cbResponse;
            centralBankLeadResponseWrapper.CorrelationId = guid;
            if (apiResult.Success)
            {
                //cbResponse = _service.GetCentralBankCreateLeadResponse(apiResult.Data!, _logger.Log);
                cbResponse = _service.GetCreateLeadResponse(apiResult.Data!, requestBody, _logger.Log);
                centralBankLeadResponseWrapper.centralBankCreateLeadResponse = cbResponse;
                //await Task.Delay(5000);
                await _sendPointInitialize.PostLeadResponse!.Send(centralBankLeadResponseWrapper);

                var response = await GetResponseObject(cbResponse);

                _logger.Info("Sending response:\n" + response.ToString());

                responseJson = response.ToString();
                stopwatch.Stop();

                var log = AuditLogFactory.CreateAuditLog(
                    correlationId: guid,
                    sourceSystem: "CentralBankAPI",
                    targetSystem: "CoreBankAPI",
                    endpoint: endPointUrl,
                    requestPayload: requestJson,
                    responsePayload: responseJson,
                    statusCode: "200",
                    requestType: MessageTypeMappings.ProductEnquiry,
                    executionTimeMs: (int)stopwatch.ElapsedMilliseconds);

                await _sendPointInitialize.AuditLog!.Send(log);
                return Ok(cbResponse);
            }
            else
            {
                centralBankLeadResponseWrapper.centralBankCreateLeadResponse = new CbPostCreateLeadResponse();
                await _sendPointInitialize.GetProductDataResponse!.Send(centralBankLeadResponseWrapper);

                var log = AuditLogFactory.CreateAuditLog(
                   correlationId: guid,
                   sourceSystem: "CentralBankAPI",
                   targetSystem: "CoreBankAPI",
                   endpoint: endPointUrl,
                   requestPayload: requestJson,
                   responsePayload: string.Empty,
                   statusCode: "200",
                   requestType: MessageTypeMappings.ProductEnquiry,
                   executionTimeMs: (int)stopwatch.ElapsedMilliseconds);

                await _sendPointInitialize.AuditLog!.Send(log);

                return NotFound(new ErrorResponse { errorCode = "404", errorMessage = apiResult.Message ?? "Customer data not found." });

            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex);

            var log = AuditLogFactory.CreateAuditLog(correlationId: guid,
                sourceSystem: "CentralBankAPI",
                targetSystem: "CoreBankAPI",
                endpoint: endPointUrl,
                requestPayload: requestJson,
                responsePayload: null,
                statusCode: "500",
                requestType: MessageTypeMappings.ProductEnquiry,
                executionTimeMs: (int)stopwatch.ElapsedMilliseconds,
                errorMessage: ex.Message);

            await _sendPointInitialize.AuditLog!.Send(log);

            return StatusCode(500, new ErrorResponse { errorCode = "500", errorMessage = "An unexpected error occurred." });

        }
        finally
        {
            stopwatch.Stop();
        }
    }


    private Task<JObject> GetResponseObject(CbPostCreateLeadResponse centralBankProductResponse)
    {
        try
        {
            var response = JObject.FromObject(centralBankProductResponse, new Newtonsoft.Json.JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            });

            return Task.FromResult(response);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred in GetResponseObject()");
            return Task.FromResult(new JObject());
        }
    }
    private CbPostCreateLeadHeader MapHeadersToCentralRequest(
     string o3ProviderId,
     string o3CallerOrgId,
     string o3CallerClientId,
     string o3CallerSoftwareStatementId,
     string o3ApiUri,
     string o3ApiOperation,
     string? o3CallerInteractionId,
     string o3OzoneInteractionId,
     string customerIpAddress,Guid correlationId

 )
    {
        return new CbPostCreateLeadHeader
        {
            CorrelationId = correlationId,
            O3ProviderId = o3ProviderId.Trim(),
            O3CallerOrgId = o3CallerOrgId.Trim(),
            O3CallerClientId = o3CallerClientId.Trim(),
            O3CallerSoftwareStatementId = o3CallerSoftwareStatementId.Trim(),
            O3ApiUri = o3ApiUri.Trim(),
            O3ApiOperation = o3ApiOperation.Trim(),
            O3CallerInteractionId = o3CallerInteractionId?.Trim(),
            O3OzoneInteractionId = o3OzoneInteractionId.Trim(),
            XFapiCustomerIpAddress= customerIpAddress.Trim()
        };
    }


}
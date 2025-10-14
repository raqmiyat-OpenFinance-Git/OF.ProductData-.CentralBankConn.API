using OF.ProductData.CentralBankConn.API.IServices;
using OF.ProductData.CentralBankConn.API.Models;
using OF.ProductData.Common.Helpers;
using OF.ProductData.Common.NLog;
using OF.ProductData.Model.CentralBank.Products;
using OF.ProductData.Model.Common;
using OF.ProductData.Model.CoreBank.Products;
using ErrorResponse = OF.ProductData.Model.Common.ErrorResponse;

namespace OF.ProductData.CentralBankConn.API.Controllers;

public class ProductDataController : ControllerBase
{
    private readonly IProductDataService _service;
    private readonly ProductLogger _logger;
    private readonly SendPointInitialize _sendPointInitialize;
    private readonly IOptions<CoreBankApis> _coreBankApis;
    private readonly IValidator<CbProductRequest> _validator;
    public ProductDataController(IProductDataService service, ProductLogger logger, SendPointInitialize sendPointInitialize, IOptions<CoreBankApis> coreBankApis, IValidator<CbProductRequest> validator)
    {
        _service = service;
        _logger = logger;
        _sendPointInitialize = sendPointInitialize;
        _coreBankApis = coreBankApis;
        _validator = validator;
    }

    [HttpGet]
    [Route("/products")]
    [ProducesResponseType(typeof(CbProductResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<IActionResult> RetrieveProduct(

      [FromHeader(Name = "authorization")] string? authorization,
     [Required][FromHeader(Name = "x-fapi-customer-ip-address")] string customerIpAddress,

    [Required][FromQuery(Name = "ProductCategory")] string? productCategory = null, // enum //SavingsAccount | CurrentAccount | CreditCard | Loan | Mortgage.
    [Required][FromQuery(Name = "IsShariaCompliant")] bool? isShariaCompliant = null,
    [Required][FromQuery(Name = "LastUpdatedDateTime")] DateTime? lastUpdatedDateTime = null,
    [Required][FromQuery(Name = "PageNumber")] int pageNumber = 1,
    [Required][FromQuery(Name = "PageSize")] int pageSize = 100,
    [Required][FromQuery(Name = "SortOrder")] string sortOrder = "asc",
    [Required][FromQuery(Name = "SortField")] string sortField = "LastUpdatedDateTime")
    {
        var stopwatch = Stopwatch.StartNew();
        authorization = authorization?.Trim();
        customerIpAddress = customerIpAddress.Trim();
        productCategory = productCategory?.Trim()!;
        isShariaCompliant = isShariaCompliant;
        lastUpdatedDateTime = lastUpdatedDateTime;
        pageNumber = pageNumber;
        pageSize = pageSize;
        sortOrder = sortOrder;
        sortField = sortField;
     

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
          

            endPointUrl = UrlHelper.CombineUrl(_coreBankApis.Value.BaseUrl!, _coreBankApis.Value.ProductServiceUrl!.GetProductUrl!);
            correlationId = HttpContext.Items["X-Correlation-ID"]?.ToString();
            if (!Guid.TryParse(correlationId, out guid))
            {
                _logger.Info($"Invalid CorrelationId: {correlationId}");
                return BadRequest(new ErrorResponse { errorCode = "400", errorMessage = $"Invalid CorrelationId: {correlationId}" });
            }
   

            CbProductRequest centralRequest = MapHeadersToCentralRequest(
            authorization, customerIpAddress, productCategory, isShariaCompliant,
            lastUpdatedDateTime, pageNumber, pageSize,
            sortOrder, sortField,  guid);

            requestJson = JsonConvert.SerializeObject(centralRequest, Formatting.Indented);

            _logger.Info($"Request received:\n{requestJson}");

            await _sendPointInitialize.GetProductDataRequest!.Send(centralRequest);

            _logger.Info($"Request send:\n{requestJson}");
         
            CbsProductRequest coreRequest = new()
            {
                //Authorization = authorization?.Trim(),
                //CustomerIpAddress = customerIpAddress.Trim(),
                //ProductCategory = productCategory?.Trim()!,
                //IsShariaCompliant = isShariaCompliant,
                //LastUpdatedDateTime = lastUpdatedDateTime,
                //PageNumber = pageNumber,
                //PageSize = pageSize,
                //SortOrder = sortOrder,
                //SortField = sortField,
                CorrelationId = guid,
            };

            // call internal API

            _logger.Info($"CoreBank Request Received: {JsonConvert.SerializeObject(coreRequest)}");
            var apiResult = await _service.GetProductFromCoreBankAsync(coreRequest);

            _logger.Info($"CoreBank Request send: {JsonConvert.SerializeObject(coreRequest)}");


            if (apiResult == null)
            {
                return NotFound(new ErrorResponse { errorCode = "404", errorMessage = "Customer data not found." });
            }
            CbProductResponseWrapper centralBankProductResponseWrapper = new();
            CbProductResponse cbResponse;
            centralBankProductResponseWrapper.CorrelationId = guid;
            if (apiResult.Success)
            {
                cbResponse = _service.GetCentralBankProductByIdResponse(apiResult.Data!, _logger.Log);
                centralBankProductResponseWrapper.centralBankProductResponse = cbResponse;
                //await Task.Delay(5000);
                await _sendPointInitialize.GetProductDataResponse!.Send(centralBankProductResponseWrapper);

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
                centralBankProductResponseWrapper.centralBankProductResponse = new CbProductResponse();
                await _sendPointInitialize.GetProductDataResponse!.Send(centralBankProductResponseWrapper);

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


    private Task<JObject> GetResponseObject(CbProductResponse centralBankProductResponse)
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
            return Task.FromResult(new JObject()); // Return empty JObject on error
        }
    }
    private CbProductRequest MapHeadersToCentralRequest(
      string authorization,
      string customerIpAddress,
      string? productCategory,
      bool? isShariaCompliant,
      DateTime? lastUpdatedDateTime,
      int pageNumber,
      int pageSize,
      string sortOrder,
      string sortField,
   
      Guid correlationId
    
  )
    {
        return new CbProductRequest
        {
            // Headers
            Authorization = authorization,
            CustomerIpAddress = customerIpAddress,

            // Query parameters
            ProductCategory = productCategory,
            IsShariaCompliant = isShariaCompliant,
            LastUpdatedDateTime = lastUpdatedDateTime,
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortOrder = sortOrder,
            SortField = sortField,
            CorrelationId = correlationId
        };
    }







}
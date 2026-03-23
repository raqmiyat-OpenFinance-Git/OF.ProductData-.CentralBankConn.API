using ConsentModel.Common;
using OF.ProductData.CentralBankConn.API.IServices;
using OF.ProductData.CentralBankConn.API.Repositories;
using OF.ProductData.Common.Custom;
using OF.ProductData.Common.Helpers;
using OF.ProductData.Common.NLog;
using OF.ProductData.Model.CentralBank.CreateLead;
using OF.ProductData.Model.CentralBank.Products;
using OF.ProductData.Model.Common;
using OF.ProductData.Model.CoreBank;
using OF.ProductData.Model.CoreBank.Products;
using Raqmiyat.Framework.Custom;
using System.Net.Http.Headers;

namespace OF.ProductData.CentralBankConn.API.Services;

public class CreateLeadService : ICreateLeadService
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<CoreBankApis> _coreBankApis;
    private readonly IMasterRepository _masterRepository;
    private readonly IOptions<ApiHeaderParams> _apiHeaderParams;
    private readonly Custom _Custom;
    private readonly ProductLogger _logger;
    public CreateLeadService(HttpClient httpClient, IOptions<CoreBankApis> coreBankApis, IMasterRepository masterRepository, IOptions<ApiHeaderParams> apiHeaderParams, Custom custom, ProductLogger logger)
    {
        _httpClient = httpClient;
        _coreBankApis = coreBankApis;
        _masterRepository = masterRepository;
        _apiHeaderParams = apiHeaderParams;
        _Custom = custom;
        _logger = logger;
    }


    public async Task<ApiResult<CbsCreateLeadResponse>> PostCreateLeadFromCoreBankAsync(CbsCreateLeadRequest cbcreateLeadRequest)
    {
        ApiResult<CbsCreateLeadResponse>? apiResult = null;

        try
        {

            _logger.Info($"GetProductFromCoreBankAsync is invoked");
            string jsonData = JsonConvert.SerializeObject(cbcreateLeadRequest, Formatting.Indented);

            _logger.Info($"CorrelationId: {cbcreateLeadRequest.CorrelationId} || JsonRequestBody: {PciDssSecurity.MaskCardInDynamicJson(jsonData, _logger.Log)}");



            var url = UrlHelper.CombineUrl(_coreBankApis.Value.BaseUrl!, _coreBankApis.Value.LeadServiceUrl!.GetLeadUrl!);

            _logger.Info($"Request Url: {url}");

            if (string.IsNullOrWhiteSpace(url))
            {
                throw new InvalidOperationException("LeadServiceUrl.GetLeadUrl is not configured.");
            }

            var content = GetStringContent(jsonData, cbcreateLeadRequest.CorrelationId.ToString());

            _httpClient.Timeout = TimeSpan.FromSeconds(30); // or pull from config
            _logger.Info($"CorrelationId: {cbcreateLeadRequest.CorrelationId} || Calling CoreBank API URL: {url}");


            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };
            var TokenapiUrl = _coreBankApis.Value.TokenUrl?.ToString();
            _logger.Info($"CorrelationId: {cbcreateLeadRequest?.CorrelationId} || Calling Token API URL: {TokenapiUrl}");
            var dynamicToken = await _Custom.PostTokenAsync(cbcreateLeadRequest?.CorrelationId.ToString()!, TokenapiUrl!, _apiHeaderParams, _logger.Log);
            request.Headers.Add("X-Correlation-ID", cbcreateLeadRequest?.CorrelationId.ToString());
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", dynamicToken.access_token);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage apiResponse = await _httpClient.SendAsync(request);

            _logger.Info($"StatusCode: {apiResponse.StatusCode}");

            string apiResponseBody = await apiResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            _logger.Info($"RawResponse: {apiResponseBody}");

            if (string.IsNullOrWhiteSpace(apiResponseBody))
            {
                throw new InvalidOperationException("CoreBank API returned empty response body.");
            }


            _logger.Info($"CorrelationId: {cbcreateLeadRequest!.CorrelationId} || Receiving Response data: {PciDssSecurity.MaskCardInDynamicJson(apiResponseBody, _logger.Log)}");

            if (apiResponse.IsSuccessStatusCode)
            {
                _logger.Info($"CorrelationId: {cbcreateLeadRequest!.CorrelationId} || API Response: SUCCESS");

                var coreBankCreateLeadResponse = JsonConvert.DeserializeObject<CbsCreateLeadResponse>(apiResponseBody) ?? throw new InvalidOperationException("Deserialized coreBankCreateLeadResponse is null.");
                apiResult = ApiResultFactory.Success(data: coreBankCreateLeadResponse!, apiResponseBody, "200");
            }
            else
            {
                _logger.Warn($"CorrelationId: {cbcreateLeadRequest!.CorrelationId}|| ExternalRefNbr: {cbcreateLeadRequest.ExternalRefNbr} || API FAILED with status {(int)apiResponse.StatusCode} - {apiResponse.ReasonPhrase}");

                apiResult = ApiResultFactory.Failure<CbsCreateLeadResponse>(apiResponseBody, ((int)apiResponse.StatusCode).ToString());
            }

        }
        catch (HttpRequestException ex) when (ex.StatusCode.HasValue)
        {
            _logger.Error(ex,
            $"Method: GetProductFromCoreBankAsync || CorrelationId: {cbcreateLeadRequest.CorrelationId.ToString() ?? "N/A"} || " +
            $"HTTP error. StatusCode: {(int)(ex.StatusCode ?? 0)} - {ex.StatusCode} || Message: {ex.Message}");

            return ApiResultFactory.Failure<CbsCreateLeadResponse>("Internal server error", "502");
        }

        catch (Exception ex)
        {
            _logger.Error(ex, $"Unhandled exception: {ex.Message}");
            return ApiResultFactory.Failure<CbsCreateLeadResponse>("Internal server error", "500");
        }

        return apiResult!;
    }

    public CbPostCreateLeadResponse GetCentralBankCreateLeadResponse(CbsCreateLeadResponse cbProductResponse, Logger logger)
    {
        var random = new Random();

        try
        {
            // ✅ Create and return dummy data
            return new CbPostCreateLeadResponse
            {
                data = new LeadResponseData
                {
                    LeadId = $"LEAD-{random.Next(100000, 999999)}",
                    Email = "user@example.com",
                    PhoneNumber = "+971501234567",
                    Name = new LeadResponseName
                    {
                        GivenName = "John",
                        LastName = "Doe"
                    },
                    EmiratesId = "784-1985-1234567-1",
                    Nationality = "AE",
                    ResidentialAddress = new LeadResponseResidentialAddress
                    {
                        AddressType = "Residential",
                        AddressLine = new List<string> { "Flat 101", "Sunset Tower" },
                        BuildingNumber = "B12",
                        BuildingName = "Sunset Tower",
                        Floor = "10",
                        StreetName = "Sheikh Zayed Road",
                        DistrictName = "Downtown",
                        PostBox = "12345",
                        TownName = "Dubai",
                        CountrySubDivision = "Dubai",
                        Country = "AE"
                    },
                    LeadInformation = "Interested in new savings account",
                    MarketingOptOut = false,
                    ProductCategories = new List<LeadResponseProductCategory>
                {
                    new LeadResponseProductCategory { Type = "SavingsAccount" },
                }
                }
            };
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error occurred in GetCentralBankCreateLeadResponse()");
            throw;
        }
    }

    public CbPostCreateLeadResponse GetCreateLeadResponse(CbsCreateLeadResponse cbProductResponse, CbPostCreateLeadRequest leadRequest, Logger logger)
    {
        var random = new Random();

        try
        {
            var productType = leadRequest.Data.ProductCategories!.Select(c => c.Type).FirstOrDefault();
            // ✅ Create and return dummy data
            return new CbPostCreateLeadResponse
            {
                data = new LeadResponseData
                {
                    LeadId = $"LEAD-{random.Next(100000, 999999)}",
                    Email = leadRequest.Data.Email,
                    PhoneNumber = leadRequest.Data.PhoneNumber,
                    Name = new LeadResponseName
                    {
                        GivenName = leadRequest.Data.Name!.GivenName,
                        LastName = leadRequest.Data.Name.LastName
                    },
                    EmiratesId = leadRequest.Data.EmiratesId,
                    Nationality = "AE",
                    ResidentialAddress = new LeadResponseResidentialAddress
                    {
                        AddressType = "Residential",
                        AddressLine = new List<string> { "Flat 101", "Sunset Tower" },
                        BuildingNumber = "B12",
                        BuildingName = "Sunset Tower",
                        Floor = "10",
                        StreetName = "Sheikh Zayed Road",
                        DistrictName = "Downtown",
                        PostBox = "12345",
                        TownName = "Dubai",
                        CountrySubDivision = "Dubai",
                        Country = "AE"
                    },
                    LeadInformation = "Interested in new " + productType,

                    MarketingOptOut = leadRequest.Data.MarketingOptOut,
                    ProductCategories = new List<LeadResponseProductCategory>
                {
                    new LeadResponseProductCategory { Type = productType },
                }
                }
            };
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error occurred in GetCentralBankCreateLeadResponse()");
            throw;
        }
    }

    private StringContent GetStringContent(string jsonContent, string correlationId)
    {
        try
        {
            return new StringContent(jsonContent, Encoding.UTF8, "application/json");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error during encryption/decryption", ex);
        }
    }

}

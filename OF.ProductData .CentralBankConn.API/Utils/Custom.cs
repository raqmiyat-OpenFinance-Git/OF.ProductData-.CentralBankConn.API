
using ConsentModel.Common;
using OF.ProductData.Model.Common;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Raqmiyat.Framework.Custom
{
    public class Custom
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<ServiceParams> _serviceParams;
        private readonly Logger _logger;
        public Custom(IOptions<ServiceParams> serviceParams, HttpClient httpClient, Logger logger)
        {
            _httpClient = httpClient;
            _serviceParams = serviceParams;
            _logger = logger;
        }
        public async Task<CommonResponse> PostTokenAsync(string IPPReferenceNbr, string endPoint, IOptions<ApiHeaderParams> _apiHeaderParam, Logger logger)
        {
            CommonResponse? dynamicTokenResponse = new CommonResponse();
            dynamicTokenResponse.ResponseCode = "9999999";
            try
            {
                if (_apiHeaderParam.Value.DynamicTokenEnabled)
                {
                    _logger.Info($" ReferenceNo: {IPPReferenceNbr} || Calling core token system.");
                    var requestData = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("client_id", _apiHeaderParam.Value.client_id!),
                        new KeyValuePair<string, string>("client_secret", _apiHeaderParam.Value.client_secret!),
                        new KeyValuePair<string, string>("channel_id", "ipp_qr_merchant")
                        });
                    _logger.Info("RequestData:" + requestData);
                    var request = new HttpRequestMessage(HttpMethod.Post, endPoint)
                    {
                        Content = requestData
                    };
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                    var handler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                    };

                    using var client = new HttpClient(handler);

                    var response = await client.SendAsync(request);

                    //var response = await _httpClient.SendAsync(request);
                    _logger.Debug($" ReferenceNo: {IPPReferenceNbr} || Sent request to token system.");
                    _logger.Debug($" ReferenceNo: {IPPReferenceNbr} || Recieved Response data :{JsonConvert.SerializeObject(response)}");
                    var responseBody = await response.Content.ReadAsStringAsync();
                    _logger.Debug($" ReferenceNo: {IPPReferenceNbr} || ReadAsStringAsync String Content: {responseBody}.");
                    if (response.IsSuccessStatusCode)
                    {
                        dynamicTokenResponse = JsonConvert.DeserializeObject<CommonResponse>(responseBody)!;
                        if (!string.IsNullOrEmpty(dynamicTokenResponse.access_token))
                        {
                            dynamicTokenResponse.ResponseContent = "SUCCESS";
                            dynamicTokenResponse.ResponseCode = "200";
                        }
                        else
                        {
                            dynamicTokenResponse.ResponseContent = $"Token response value comes empty or null from core api.";
                        }
                    }
                    else
                    {
                        _logger.Error($" ReferenceNo: {IPPReferenceNbr} || Failed to fetch token. Status code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
                        dynamicTokenResponse.ResponseContent = $"Failed to fetch token. Status code: {response.StatusCode}, Reason: {response.ReasonPhrase}";
                    }
                }
                else
                {
                    dynamicTokenResponse.ResponseContent = "SUCCESS";
                    dynamicTokenResponse.ResponseCode = "200";
                    dynamicTokenResponse.access_token = _apiHeaderParam.Value.AuthToken;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $" ReferenceNo: {IPPReferenceNbr} || An error occurred while fetching the token:  {ex.Message}" );
                dynamicTokenResponse.ResponseContent = ex.Message;
            }
            return dynamicTokenResponse;
        }


    }
}

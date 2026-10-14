using ConsentModel.Common;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;
using OF.ProductData.Common.NLog;
using OF.ProductData.Model.Common;
using System.Net.Http.Headers;

namespace ConsentCoreBankAPI.Services
{
    public class CustomTokenApiService(HttpClient httpClient, IOptions<ApiHeaderParams> apiHeaderParams) : ICustomTokenApiService
    {
        private readonly ApiHeaderParams _apiHeaderParam = apiHeaderParams.Value;
        public async Task<CommonResponse> PostTokenAsync(string IPPReferenceNbr, string endPoint, BaseLogger logger)
        {
            string logPrefix = string.Empty;
            logPrefix = $"ReferenceNo: {IPPReferenceNbr} || ";

            CommonResponse? dynamicTokenResponse = new CommonResponse();
            dynamicTokenResponse.ResponseCode = "9999999";
            try
            {
                if (_apiHeaderParam.DynamicTokenEnabled)
                {
                    logger.Debug($"{logPrefix}Calling core token system.");
                    var requestData = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("grant_type", _apiHeaderParam.grant_type!),
                        new KeyValuePair<string, string>("client_id", _apiHeaderParam.client_id!),
                        new KeyValuePair<string, string>("client_secret", _apiHeaderParam.client_secret!)
                        });
                    var request = new HttpRequestMessage(HttpMethod.Post, endPoint)
                    {
                        Content = requestData
                    };
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                    var response = await httpClient.SendAsync(request);
                    logger.Debug($"{logPrefix}Sent request to token system.");
                    logger.Debug($"{logPrefix}Recieved Response data :{JsonConvert.SerializeObject(response)}");
                    var responseBody = await response.Content.ReadAsStringAsync();
                    logger.Debug($"{logPrefix}ReadAsStringAsync String Content: {responseBody}.");
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
                        logger.Error($"{logPrefix}Failed to fetch token. Status code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
                        dynamicTokenResponse.ResponseContent = $"Failed to fetch token. Status code: {response.StatusCode}, Reason: {response.ReasonPhrase}";
                    }
                }
                else
                {
                    dynamicTokenResponse.ResponseContent = "SUCCESS";
                    dynamicTokenResponse.ResponseCode = "200";
                    dynamicTokenResponse.access_token = _apiHeaderParam.AuthToken;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{logPrefix}An error occurred while fetching the token:  {ex.Message}");
                dynamicTokenResponse.ResponseContent = ex.Message;
            }
            return dynamicTokenResponse;
        }
    }
    public interface ICustomTokenApiService
    {
        Task<CommonResponse> PostTokenAsync(string IPPReferenceNbr, string endPoint, BaseLogger logger);
    }
}

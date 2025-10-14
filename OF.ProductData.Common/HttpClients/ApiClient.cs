namespace OF.ProductData.Common.ApiClient;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly Logger _logger;
    

    public ApiClient(HttpClient httpClient, Logger logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(TRequest request, string url)
    {
        try
        {
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);

            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.Error($"PostToCoreBankAsync failed: {response.StatusCode} - {result}");
                return default;
            }

            var deserialized = JsonConvert.DeserializeObject<TResponse>(result);
            if (deserialized == null)
            {
                _logger.Error("Failed to deserialize response.");
                return default;
            }

            return deserialized;
        }
        catch (Exception ex)
        {
            _logger.Error(ex);
            return default;
        }
    }
    public async Task<TResponse?> GetAsync<TResponse>(string url)
    {
        try
        {
            var response = await _httpClient.GetAsync(url);

            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.Error($"GetFromBankApiAsync failed: {response.StatusCode} - {result}");
                return default;
            }

            var deserialized = JsonConvert.DeserializeObject<TResponse>(result);
            if (deserialized == null)
            {
                _logger.Error("Failed to deserialize GET response.");
                return default;
            }

            return deserialized;
        }
        catch (Exception ex)
        {
            _logger.Error(ex);
            return default;
        }
    }

}

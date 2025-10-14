namespace OF.ProductData.Common.Helpers;

public static class JsonHelper
{
    private static readonly JsonSerializerOptions _defaultOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// Serializes an object to a JSON string using default options.
    /// </summary>
    public static string Serialize<T>(T obj, JsonSerializerOptions? options = null)
    {
        return System.Text.Json.JsonSerializer.Serialize(obj, options ?? _defaultOptions);
    }

    /// <summary>
    /// Deserializes a JSON string to an object of type T using default options.
    /// </summary>
    public static T? Deserialize<T>(string json, JsonSerializerOptions? options = null)
    {
        return System.Text.Json.JsonSerializer.Deserialize<T>(json, options ?? _defaultOptions);
    }

    /// <summary>
    /// Try to deserialize without throwing exception. Returns success status and object.
    /// </summary>
    public static bool TryDeserialize<T>(string json, out T? result, JsonSerializerOptions? options = null)
    {
        try
        {
            result = System.Text.Json.JsonSerializer.Deserialize<T>(json, options ?? _defaultOptions);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }
}

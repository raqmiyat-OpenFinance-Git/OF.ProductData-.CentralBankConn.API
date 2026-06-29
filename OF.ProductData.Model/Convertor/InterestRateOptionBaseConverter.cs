using OF.ProductData.Model.CentralBank.Products;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OF.ProductData.Model.Convertor;

internal class InterestRateOptionBaseConverter : JsonConverter<InterestRateOptionBase>
{
    public override InterestRateOptionBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDocument = JsonDocument.ParseValue(ref reader);
        var root = jsonDocument.RootElement;

        if (!root.TryGetProperty("RateType", out var rateTypeProperty))
        {
            throw new JsonException("RateType is required.");
        }

        var rateType = rateTypeProperty.GetString();

        if (string.IsNullOrWhiteSpace(rateType))
        {
            throw new JsonException("RateType cannot be null or empty.");
        }

        var json = root.GetRawText();

        switch (rateType.Trim().ToLowerInvariant())
        {
            case "fixedinterest":
                return JsonSerializer.Deserialize<FixedInterestRateOption>(json, options);

            case "variableinterest":
                return JsonSerializer.Deserialize<VariableInterestRateOption>(json, options);

            case "hybridinterest":
                return JsonSerializer.Deserialize<HybridInterestRateOption>(json, options);

            default:
                throw new JsonException($"Unsupported Interest RateType '{rateType}'.");
        }
    }

    public override void Write(Utf8JsonWriter writer, InterestRateOptionBase value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
    }
}

using OF.ProductData.Model.CentralBank.Products;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OF.ProductData.Model.Convertor;

internal class ProfitRateOptionBaseConverter : JsonConverter<ProfitRateOptionBase>
{
    public override ProfitRateOptionBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
            case "fixedprofit":
                return JsonSerializer.Deserialize<FixedProfitRateOption>(json, options);

            case "variableprofit":
                return JsonSerializer.Deserialize<VariableProfitRateOption>(json, options);

            case "hybridprofit":
                return JsonSerializer.Deserialize<HybridProfitRateOption>(json, options);

            default:
                throw new JsonException($"Unsupported Profit RateType '{rateType}'.");
        }
    }

    public override void Write(Utf8JsonWriter writer, ProfitRateOptionBase value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
    }
}

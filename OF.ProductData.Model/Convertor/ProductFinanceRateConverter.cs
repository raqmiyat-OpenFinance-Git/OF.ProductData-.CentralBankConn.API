using OF.ProductData.Model.CentralBank.Products;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace OF.ProductData.Model.Convertor;

internal class ProductFinanceRateConverter : JsonConverter<ProductFinanceRate>
{
    public override ProductFinanceRate? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
                return JsonSerializer.Deserialize<FixedInterest>(json, options);

            case "fixedprofit":
                return JsonSerializer.Deserialize<FixedProfit>(json, options);

            case "variableinterest":
                return JsonSerializer.Deserialize<VariableInterest>(json, options);

            case "variableprofit":
                return JsonSerializer.Deserialize<VariableProfit>(json, options);

            case "hybridinterest":
                return JsonSerializer.Deserialize<HybridInterest>(json, options);

            case "hybridprofit":
                return JsonSerializer.Deserialize<HybridProfit>(json, options);

            case "interestrateoptions":
                return JsonSerializer.Deserialize<InterestRateOptions>(json, options);

            case "profitrateoptions":
                return JsonSerializer.Deserialize<ProfitRateOptions>(json, options);

            default:
                throw new JsonException($"Unsupported RateType '{rateType}'.");
        }
    }

    public override void Write(Utf8JsonWriter writer, ProductFinanceRate value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
    }
}

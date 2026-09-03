using System.Text.Json;

namespace OF.ProductData.Model.Convertor
{
    public static class JsonElementExtensions
    {
        public static bool TryGetPropertyCaseInsensitive(this JsonElement element,string propertyName, out JsonElement value)
        {
            foreach (var prop in element.EnumerateObject())
            {
                if (string.Equals(prop.Name, propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    value = prop.Value;
                    return true;
                }
            }

            value = default;
            return false;
        }
    }
}

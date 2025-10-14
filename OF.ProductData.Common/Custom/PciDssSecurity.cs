namespace OF.ProductData.Common.Custom;

public static class PciDssSecurity
{
    public static string MaskCardInDynamicJson(string json, Logger logger)
    {
        if (string.IsNullOrWhiteSpace(json))
            return json;
        try
        {
            var jObj = JObject.Parse(json);

            // Fields to check (case-insensitive)
            var fieldsToMask = new[]
            {
                "beneficiaryaccountnumber",
                "orderingcustomeraccountnumber",
                "posting_dr_acct",
                "posting_cr_acct",
                "iban",
                "ibanoraccountnumber",
                "debtoriban",
                "beneficiarycustomeraccountnumber",

             };

            foreach (var field in fieldsToMask)
            {
                var prop = jObj.Properties()
                    .FirstOrDefault(p => string.Equals(p.Name.ToLower(), field, StringComparison.OrdinalIgnoreCase));

                if (prop != null)
                {
                    string value = Convert.ToString(prop.Value) ?? string.Empty;

                    // Only mask if it's exactly 16 digits
                    if (value.Length == 16 && Regex.IsMatch(value, @"^\d{16}$"))
                    {
                        // Mask 4 digits before the last 4
                        string masked = value.Substring(0, 8) + "****" + value.Substring(12, 4);
                        prop.Value = masked;
                    }
                }
            }

            return jObj.ToString();
        }
        catch (Exception ex)
        {
            logger.Error(ex, $"An error occurred while MaskCardInDynamicJson. Exception details: {ex}");
        }
        return json;
    }
    public static string MaskAccountIdInXml(string xml, Logger logger)
    {
        if (string.IsNullOrWhiteSpace(xml))
            return xml;

        try
        {
            var doc = XDocument.Parse(xml);

            // Targeting <Id> under <Othr> inside <DbtrAcct> or <CdtrAcct>
            var accountTypes = new[] { "DbtrAcct", "CdtrAcct" };

            foreach (var acctType in accountTypes)
            {
                var acctElements = doc.Descendants()
                    .Where(e => string.Equals(e.Name.LocalName, acctType, StringComparison.OrdinalIgnoreCase));

                foreach (var acct in acctElements)
                {
                    var idElement = acct
                        .Descendants()
                        .FirstOrDefault(e =>
                            string.Equals(e.Name.LocalName, "Id", StringComparison.OrdinalIgnoreCase) &&
                            e.Parent?.Name.LocalName.Equals("Othr", StringComparison.OrdinalIgnoreCase) == true);

                    if (idElement != null)
                    {
                        string value = idElement.Value;
                        if (value.Length == 16 && Regex.IsMatch(value, @"^\d{16}$"))
                        {
                            idElement.Value = value.Substring(0, 8) + "****" + value.Substring(12, 4);
                        }
                    }
                }
            }

            return doc.ToString();
        }
        catch (Exception ex)
        {
            logger.Error(ex, $"Error while masking account Ids: {ex}");
            return xml;
        }
    }

}

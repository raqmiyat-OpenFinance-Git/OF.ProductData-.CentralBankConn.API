namespace OF.ProductData.Model.CentralBank.Products;
public class CbPostCreateLeadResponse
{
    public LeadResponseData Data { get; set; } = new LeadResponseData();
}

public class LeadResponseData
{
    public string? LeadId { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public LeadResponseName? Name { get; set; }
    public string? EmiratesId { get; set; }
    public string? Nationality { get; set; }
    public LeadResponseResidentialAddress? ResidentialAddress { get; set; }
    public string? LeadInformation { get; set; }
    public bool? MarketingOptOut { get; set; }
    public List<LeadResponseProductCategory>? ProductCategories { get; set; }
}

public class LeadResponseName
{
    public string? GivenName { get; set; }
    public string? LastName { get; set; }
}

public class LeadResponseResidentialAddress
{
    public string? AddressType { get; set; }
    public List<string>? AddressLine { get; set; }
    public string? BuildingNumber { get; set; }
    public string? BuildingName { get; set; }
    public string? Floor { get; set; }
    public string? StreetName { get; set; }
    public string? DistrictName { get; set; }
    public string? PostBox { get; set; }
    public string? TownName { get; set; }
    public string? CountrySubDivision { get; set; }
    public string? Country { get; set; }
}

public class LeadResponseProductCategory
{
    public string? Type { get; set; }
}

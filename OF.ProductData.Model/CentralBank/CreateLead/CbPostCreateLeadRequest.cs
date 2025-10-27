namespace OF.ProductData.Model.CentralBank.CreateLead;


    public class CbPostCreateLeadRequest
    {
        public LeadData Data { get; set; } = new LeadData();
    }

    public class LeadData
    {
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public LeadName? Name { get; set; }
        public string? EmiratesId { get; set; }
        public string? Nationality { get; set; }
        public ResidentialAddress? ResidentialAddress { get; set; }
        public string? LeadInformation { get; set; }
        public bool? MarketingOptOut { get; set; }
        public List<ProductCategory>? ProductCategories { get; set; }
    }

    public class LeadName
    {
        public string? GivenName { get; set; }
        public string? LastName { get; set; }
    }

    public class ResidentialAddress
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
    public class ProductCategory
    {
    public string? Type { get; set; }  // e.g. "SavingsAccount", "CurrentAccount", etc.
    }

    


namespace OF.ProductData.Model.EFModel.Products;

[Table("Lfi_LeadRequest")]
public class EFCreateLeadRequest
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long RequestId { get; set; }
    public Guid CorrelationId { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? GivenName { get; set; }
    public string? LastName { get; set; }
    public string? EmiratesId { get; set; }
    public string? Nationality { get; set; }
    public string? LeadResidentialAddressType { get; set; }
    public string? LeadResidentialAddressLine { get; set; } // comma-separated if multiple line
    public string? LeadResidentialBuildingNumber { get; set; }
    public string? LeadResidentialBuildingName { get; set; }
    public string? LeadResidentialFloor { get; set; }
    public string? LeadResidentialStreetName { get; set; }
    public string? LeadResidentialDistrictName { get; set; }
    public string? LeadResidentialPostBox { get; set; }
    public string? LeadResidentialTownName { get; set; }
    public string? LeadResidentialCountrySubDivision { get; set; }
    public string? LeadResidentialCountry { get; set; }
    public string? ProductType { get; set; }
    public string? LeadInformation { get; set; }
    public bool? MarketingOptOut { get; set; }
    public string? Status { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public string? RequestJson { get; set; }
    public ICollection<EFCreateLeadHeaderRequest>? createLeadHeaderRequest { get; set; }
}

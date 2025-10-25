namespace OF.ProductData.Model.EFModel.Products
{
    [Table("Lfi_LeadResponse")]
    public class EFCreateLeadResponse
    {
   
        [Key]
        public long Id { get; set; }  

        [ForeignKey("createLeadRequest")]
        public long RequestId { get; set; }
        public string? LeadId { get; set; }      
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }      
        public string? GivenName { get; set; }
        public string? LastName { get; set; }      
        public string? EmiratesId { get; set; }    
        public string? Nationality { get; set; }
        public string? LeadResidentiaAddressType { get; set; }
        public string? LeadResidentialAddressAddressLine { get; set; }
        public string? LeadResidentialAddressBuildingNumber { get; set; }
        public string? LeadResidentialAddressBuildingName { get; set; }
        public string? LeadResidentialAddressFloor { get; set; }
        public string? LeadResidentialAddressStreetName { get; set; }
        public string? LeadResidentialAddressDistrictName { get; set; }
        public string? LeadResidentialAddressPostBox { get; set; }   
        public string? LeadResidentialAddressTownName { get; set; }
        public string? LeadResidentialAddressCountrySubDivision { get; set; }
        public string? LeadResidentialAddressCountry { get; set; }
        public string? LeadInformation { get; set; } 
        public bool? MarketingOptOut { get; set; }  
        public string? ProductType { get; set; }
        public string? Status { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ResponseJson { get; set; }  
        public EFCreateLeadRequest? createLeadRequest { get; set; }
    }
}

namespace OF.ProductData.Model.EFModel.Products
{
    [Table("ProductDataResponse")]

    public class EFProductResponse
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [ForeignKey("ProductRequest")]
        public long RequestId { get; set; }
        public string? LFIId { get; set; }
        public string? LFIBrandId { get; set; }
        public DateTime LastUpdatedDateTime { get; set; }
        public bool IsShariaCompliant { get; set; }
        public string? ShariaInformation { get; set; }
        public bool IsSalaryTransferRequired { get; set; }

        public string? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductCategory { get; set; }
        public string? Description { get; set; }
        public DateTime EffectiveFromDateTime { get; set; }
        public DateTime EffectiveToDateTime { get; set; }

        public string? ApplicationUri { get; set; }
        public string? ApplicationEmail { get; set; }
        public string? ApplicationPhoneNumber { get; set; }
        public string? ApplicationDescription { get; set; }
        public string? KfsUri { get; set; }
        public string? OverviewUri { get; set; }
        public string? TermsUri { get; set; }
        public string? FeesAndPricingUri { get; set; }
        public string? ScheduleOfChargesUri { get; set; }
        public string? EligibilityUri { get; set; }
        public string? CardImageUri { get; set; }
        public string? ChannelsType { get; set; }
        public string? ChannelsDescription { get; set;}
        public string? ResidenceStatusType { get; set; }
        public string? ResidenceStatusDescription { get; set; }
        public string? EmploymentStatusType { get; set; }
        public string? EmploymentStatusDescription { get; set; }
        public string? CustomerTypeType { get; set; }
        public string? CustomerTypeDescription { get; set; }
        public string? AccountOwnershipType { get; set; }
        public string? AccountOwnershipDescription { get; set; }
        public string? AgeEligibilityType { get; set; }
        public string? AgeEligibilityDescription { get; set; }
        public decimal? AgeEligibilityValue { get; set; }
        public string? AdditionalEligibilityType { get; set; }
        public string? AdditionalEligibilityDescription { get; set; }
        public string? CreatedBy { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ResponsePayload { get; set; }

        public virtual ICollection<CurrentAccounts>? CurrentAccount { get; set; }
        public virtual ICollection<SavingsAccount>? SavingsAccount { get; set; }
        public virtual ICollection<CreditCard>? CreditCard { get; set; }
        public virtual ICollection<PersonalLoan>? PersonalLoan { get; set; }
        public virtual ICollection<Mortgage>? Mortgage { get; set; }
        public virtual ICollection<ProfitSharingRate>? ProfitSharingRate { get; set; }
        public virtual ICollection<FinanceProfitRate>? FinanceProfitRate { get; set; }

        public EFProductRequest? ProductRequest { get; set; }
    }
}

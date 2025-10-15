using System.Text.Json.Serialization;

namespace OF.ProductData.Model.CentralBank.Products;
public class CbProductDataResponse
{
    public List<LFIData>? Data { get; set; }
}
public class LFIData
{
    public string? LFIId { get; set; }
    public string? LFIBrandId { get; set; }
    public List<ProductWrapper>? Products { get; set; }
}

public class ProductWrapper
{
    public string? ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductCategory { get; set; }
    public string? Description { get; set; }
    public DateTime EffectiveFromDateTime { get; set; }
    public DateTime EffectiveToDateTime { get; set; }
    public DateTime LastUpdatedDateTime { get; set; }
    public bool IsShariaCompliant { get; set; }
    public string? ShariaInformation { get; set; }
    public bool IsSalaryTransferRequired { get; set; }
    public Links? Links { get; set; }
    public EligibilityData? Eligibility { get; set; }
    public List<Channel>? Channels { get; set; }
    public ProductDetails? Product { get; set; }
}

public class Links
{
    public string? ApplicationUri { get; set; }
    public string? ApplicationPhoneNumber { get; set; }
    public string? ApplicationEmail { get; set; }
    public string? ApplicationDescription { get; set; }
    public string? KfsUri { get; set; }
    public string? OverviewUri { get; set; }
    public string? TermsUri { get; set; }
    public string? FeesAndPricingUri { get; set; }
    public string? ScheduleOfChargesUri { get; set; }
    public string? EligibilityUri { get; set; }
    public string? CardImageUri { get; set; }
}

public class EligibilityData
{
    public List<TypeDescription>? ResidenceStatus { get; set; }
    public List<TypeDescription>? EmploymentStatus { get; set; }
    public List<TypeDescription>? CustomerType { get; set; }
    public List<TypeDescription>? AccountOwnership { get; set; }
    public List<AgeEligibility>? Age { get; set; }
    public List<TypeDescription>? AdditionalEligibility { get; set; }
}

public class TypeDescription
{
    public string? Type { get; set; }
    public string? Description { get; set; }
}

public class AgeEligibility : TypeDescription
{
    public decimal Value { get; set; }
}

public class Channel
{
    public string? Type { get; set; }
    public string? Description { get; set; }
}

public class ProductDetails
{
    public CurrentAccountData? CurrentAccount { get; set; }
    public SavingsAccountData? SavingsAccount { get; set; }
    public CreditCardData? CreditCard { get; set; }
    public PersonalLoanData? PersonalLoan { get; set; }
    public MortgageData? Mortgage { get; set; }
    public ProfitSharingRateData? ProfitSharingRate { get; set; }
    public FinanceProfitRateData? FinanceProfitRate { get; set; }
}

public class CurrentAccountData : AccountBase { }
public class SavingsAccountData : AccountBase
{
    public Amount? MinimumBalance { get; set; }
    public double AnnualReturn { get; set; }
}
public class CreditCardData : AccountBase
{
    public decimal Rate { get; set; }
}
public class PersonalLoanData : AccountBase
{
    public Amount? MinimumLoanAmount { get; set; }
    public Amount? MaximumLoanAmount { get; set; }
    public LoanTenure? Tenure { get; set; }
    public string? CalculationMethod { get; set; }
    public RateDetails? Rate { get; set; }
    public APR? AnnualPercentageRateRange { get; set; }
    public string? FixedRatePeriod { get; set; }
    public string? DebtBurdenRatio { get; set; }
    public List<AdditionalInformation>? AdditionalInformation { get; set; }
}
public class MortgageData : PersonalLoanData
{
    public string? Structure { get; set; }
    public double MaximumLTV { get; set; }
    public Amount? DownPayment { get; set; }
    public APR? IndicativeAPR { get; set; }
}

public class ProfitSharingRateData
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Amount? MinimumDepositAmount { get; set; }
    public decimal AnnualReturn { get; set; }
    public List<NameDescription>? AnnualReturnOptions { get; set; }
    public NameDescription? InvestmentPeriod { get; set; }
    public List<AdditionalInformation>? AdditionalInformation { get; set; }
}

public class FinanceProfitRateData
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? CalculationMethod { get; set; }
    public decimal Rate { get; set; }
    public string? Frequency { get; set; }
    public List<Tier>? Tiers { get; set; }
    public List<AdditionalInformation>? AdditionalInformation { get; set; }
}

public class AccountBase
{
    public string? Type { get; set; }
    public string? Description { get; set; }
    public bool IsOverdraftAvailable { get; set; }
    public List<Document>? Documentation { get; set; }
    public List<Feature>? Features { get; set; }
    public List<Fee>? Fees { get; set; }
    public List<Limit>? Limits { get; set; }
    public List<Benefit>? Benefits { get; set; }
}

public class Document
{
    public string? Type { get; set; }
    public string? Description { get; set; }
}

public class Feature : Document { }

public class Fee
{
    public string? Type { get; set; }
    public string? Period { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Unit { get; set; }
    public Amount? Amount { get; set; }
    public double Percentage { get; set; }
    public double UnitValue { get; set; }
    public double MaximumUnitValue { get; set; }
}

public class Limit
{
    public string? Type { get; set; }
    public string? Description { get; set; }
    public double Value { get; set; }
    public double? Percentage { get; set; }
}

public class Benefit : Limit
{
    public string? Name { get; set; }
}

public class Amount
{
    [JsonPropertyName("Amount")]
    public string? AmountValue { get; set; }
    public string? Currency { get; set; }
}

public class LoanTenure
{
    public decimal MinimumLoanTenure { get; set; }
    public decimal MaximumLoanTenure { get; set; }
}

public class RateDetails
{
    public string? Type { get; set; }
    public string? Description { get; set; }
    public string? ReviewFrequency { get; set; }
    public APR? IndicativeRate { get; set; }
    public APR? ProfitRate { get; set; }
}

public class APR
{
    public decimal From { get; set; }
    public decimal To { get; set; }
}

public class AdditionalInformation : TypeDescription { }
public class NameDescription
{

    public string? Name { get; set; }


    public string? Description { get; set; }
}

public class Tier 
{

    public string? Type { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    public string? Unit { get; set; }


    public Amount? MinimumTierValue { get; set; }


    public Amount? MaximumTierValue { get; set; }

    
    public decimal MinimumTierRate { get; set; }


    public decimal MaximumTierRate { get; set; }


    public string? Condition { get; set; }
}

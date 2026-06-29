using OF.ProductData.Model.Convertor;
using System.Text.Json.Serialization;

namespace OF.ProductData.Model.CentralBank.Products;
public class CbProductDataResponse
{
    public List<LFIData>? Data { get; set; }
    public LFIMeta? Meta { get; set; }
}
public class LFIData
{
    public string? LFIId { get; set; }
    public string? LFIBrandId { get; set; }
    public List<ProductWrapper>? Products { get; set; }
}
public class LFIMeta
{
    public int? TotalPages { get; set; }
    public int? TotalRecords { get; set; }
}
public class ProductWrapper
{
    public string? ProductId { get; set; }
    public string? ProductName { get; set; }
    public ProductCategory? ProductCategory { get; set; }
    public string? Description { get; set; }
    public DateTime EffectiveFromDateTime { get; set; }
    public DateTime EffectiveToDateTime { get; set; }
    public DateTime LastUpdatedDateTime { get; set; }
    public bool IsShariaCompliant { get; set; }
    public ShariaStructure? ShariaStructure { get; set; }
    public string? AlternativeBrandName { get; set; }
    public string? ShariaInformation { get; set; }
    public bool IsSalaryTransferRequired { get; set; }
    public Links? Links { get; set; }
    public EligibilityData? Eligibility { get; set; }
    public List<Channel>? Channels { get; set; }
    public ProductDetails? Product { get; set; }
    public string? DenominationCurrency { get; set; }
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
    public List<ResidenceStatusItem>? ResidenceStatus { get; set; }
    public List<EmploymentStatusItem>? EmploymentStatus { get; set; }
    public List<CustomerTypeItem>? CustomerType { get; set; }
    public List<AccountOwnershipItem>? AccountOwnership { get; set; }
    public List<AgeEligibilityItem>? Age { get; set; }
    public List<FinancialRequirementItem>? FinancialRequirements { get; set; }
    public List<AdditionalEligibilityItem>? AdditionalEligibility { get; set; }
}
public class ResidenceStatusItem
{
    public ResidenceStatusType Type { get; set; }
    public string? Description { get; set; }
}

public class EmploymentStatusItem
{
    public EmploymentStatusType Type { get; set; }
    public string? Description { get; set; }
}

public class CustomerTypeItem
{
    public CustomerType Type { get; set; }
    public string? Description { get; set; }
}


public class AccountOwnershipItem
{
    public AccountOwnershipType Type { get; set; }
    public string? Description { get; set; }
}

public class AgeEligibilityItem
{
    public AgeType Type { get; set; }
    public string? Description { get; set; }
    public decimal Value { get; set; }
}

public class FinancialRequirementItem
{
    public FinancialRequirementType Type { get; set; }
    public string? Description { get; set; }
    public decimal? Value { get; set; }
    public Amount? Amount { get; set; }
}

public class AdditionalEligibilityItem
{
    public AdditionalEligibilityType Type { get; set; }
    public string? Description { get; set; }
}

public class AdditionalInformationItem
{
    public AdditionalInformationType? Type { get; set; }
    public string? Description { get; set; }
}

public class Channel
{
    public ChannelType? Type { get; set; }
    public string? Description { get; set; }
}

public class ProductDetails
{
    public CurrentAccountData? CurrentAccount { get; set; }
    public SavingsAccountData? SavingsAccount { get; set; }
    public CreditCardData? CreditCard { get; set; }
    public FinanceData? Finance { get; set; }
    public MortgageData? Mortgage { get; set; }
    public DepositRatesData? DepositRates { get; set; }
    public ProductFinanceRate? FinanceRates { get; set; }
    public List<Tenor>? Tenor { get; set; }
    public List<AssetBacked>? AssetBacked { get; set; }
    public List<RewardsBenefits>? RewardsBenefits { get; set; }
}


public class CurrentAccountData
{
    public CurrentAccountType? Type { get; set; }
    public bool IsOverdraftAvailable { get; set; }
    public List<Document>? Documentation { get; set; }
    public List<CurrentAccountFeature>? Features { get; set; }
    public List<ProductCharge>? Charges { get; set; }
    public List<CurrentAccountLimit>? Limits { get; set; }
}
public class SavingsAccountData 
{
    public SavingsAccountType? Type { get; set; }
    public Amount? MinimumBalance { get; set; }
    public List<Document>? Documentation { get; set; }
    public List<SavingsAccountFeature>? Features { get; set; }
    public List<ProductCharge>? Charges { get; set; }
    public List<SavingsAccountLimit>? Limits { get; set; }
}
public class CreditCardData
{
    public CreditCardType? Type { get; set; }
    public List<Document>? Documentation { get; set; }
    public List<CreditCardFeature>? Features { get; set; }
    public List<ProductCharge>? Charges { get; set; }
    public List<CreditCardLimit>? Limits { get; set; }
}
public class FinanceData 
{
    public FinanceType? Type { get; set; }
    public Amount? MinimumFinanceAmount { get; set; }
    public Amount? MaximumFinanceAmount { get; set; }
    public List<Document>? Documentation { get; set; }
    public List<FinanceFeature>? Features { get; set; }
    public List<FinanceLimit>? Limits { get; set; }
    public List<ProductCharge>? Charges { get; set; }
    public List<AdditionalInformationItem>? AdditionalInformation { get; set; }
}

public class DownPayment
{
    public string? CustomerCategory { get; set; }
    public decimal MinimumPercent { get; set; }
    public string? Basis { get; set; }
}

public class MortgageData 
{
    public Amount? MinimumFinanceAmount { get; set; }
    public Amount? MaximumFinanceAmount { get; set; }
    public List<DownPaymentRequirement>? DownPayment { get; set; }
    public List<Document>? Documentation { get; set; }
    public List<MortgageFeature>? Features { get; set; }
    public List<ProductCharge>? Charges { get; set; }
    public List<MortgageLimit>? Limits { get; set; }
}
public class DownPaymentRequirement
{
    public string? CustomerCategory { get; set; }
    public decimal MinimumPercent { get; set; }
    public string? Basis { get; set; }
}

public class ProfitSharingRateData
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Amount? MinimumDepositAmount { get; set; }
    public decimal AnnualReturn { get; set; }
    public List<NameDescription>? AnnualReturnOptions { get; set; }
    public NameDescription? InvestmentPeriod { get; set; }
    public List<AdditionalInformationItem>? AdditionalInformation { get; set; }
}

public class BaseProductDetails
{
    public CurrentAccountType? Type { get; set; }
    public bool IsOverdraftAvailable { get; set; }
    public List<Document>? Documentation { get; set; }
    public List<CurrentAccountFeature>? Features { get; set; }
    public List<ProductCharge>? Charges { get; set; }

    public List<CurrentAccountLimit>? Limits { get; set; }
}
public class BaseAccountDetails : BaseProductDetails
{
    public Amount? MinimumBalance { get; set; }
}


public class BaseLimit
{
    public string? Description { get; set; }
    public Amount? Amount { get; set; }
    public decimal Value { get; set; }
    public decimal? Percentage { get; set; }
}

public class CurrentAccountLimit 
{
    public LimitTypeCurrentAccount Type { get; set; }
    public string? Description { get; set; }
    public Amount? Amount { get; set; }
    public decimal Value { get; set; }
}
public class SavingsAccountLimit 
{
    public LimitTypeSavingsAccount Type { get; set; }
    public string? Description { get; set; }
    public Amount? Amount { get; set; }
    public decimal Value { get; set; }
}

public class CreditCardLimit 
{
    public LimitTypeCreditCard Type { get; set; }
    public string? Description { get; set; }
    public Amount? Amount { get; set; }
    public decimal Value { get; set; }
}

public class FinanceLimit : BaseLimit
{
    public LimitTypeFinance Type { get; set; }
    public string? Description { get; set; }
    public Amount? Amount { get; set; }
    public decimal Value { get; set; }
}

public class MortgageLimit : BaseLimit
{
    public LimitTypeMortgage Type { get; set; }
    public string? Description { get; set; }
    public Amount? Amount { get; set; }
    public decimal Value { get; set; }
    public decimal Percentage { get; set; }
}

public class CurrentAccountFeature
{
    public string? Description { get; set; }
    public FeatureTypeCurrentAccount Type { get; set; }
}

public class SavingsAccountFeature
{
    public string? Description { get; set; }
    public FeatureTypeSavingsAccount Type { get; set; }
}

public class CreditCardFeature
{
    public string? Description { get; set; }
    public FeatureTypeCreditCard Type { get; set; }
}
public class FinanceFeature
{
    public string? Description { get; set; }
    public FeatureTypeFinance Type { get; set; }
}

public class MortgageFeature
{
    public string? Description { get; set; }
    public FeatureTypeMortgage Type { get; set; }
}

public class Document
{
    public DocumentType? Type { get; set; }
    public string? Description { get; set; }
}

public class Feature
{
    public string? Type { get; set; }
    public string? Description { get; set; }
}

public class ChargeComponent
{
    public Amount? Amount { get; set; }
    public decimal? Rate { get; set; }
    public string? ApplicationFrequency { get; set; }
    public string? InterestCalculationMethod { get; set; }
    public Amount? MaximumChargeAmount { get; set; }
    public string? Basis { get; set; }
}
public class ProductCharge
{
    public ChargeType? Type { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public List<ChargeComponent>? Charge { get; set; }
    public List<Condition>? Conditions { get; set; }
    public string? Justification { get; set; }
    public string? Frequency { get; set; }
    public bool? DonatedToCharity { get; set; }
    public string? Notes { get; set; }
    public object? SupplementaryInformation { get; set; }
}
public class Charges : Document
{
    public string? Name { get; set; }
    public List<ProductCharge>? Charge { get; set; }
    public string? Rate { get; set; }
    public string? ApplicationFrequency { get; set; }
    public string? InterestCalculationMethod { get; set; }
    public List<ProductCharge>? MaximumChargeAmount { get; set; }
    public string? Basis { get; set; }
    public List<Conditions>? Conditions { get; set; }
    public string? Justification { get; set; }
    public string? Frequency { get; set; }
    public bool DonatedToCharity { get; set; }
    public string? Notes { get; set; }
    public string? SupplementaryInformation { get; set; }
}

public class Conditions     
{
    public string? Field { get; set; }
    public string? Operator { get; set; }
    public string? Value { get; set; }
    public string? Description { get; set; }
}

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
    public Amount? Amount { get; set; }
    public decimal Value { get; set; }
    public decimal? Percentage { get; set; }
}

public class Benefit : Limit
{
    public string? Name { get; set; }
}

public class Amount
{
    [JsonPropertyName("Amount")]
    public decimal? AmountValue { get; set; }
    [JsonPropertyName("Currency")]
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
public class NameDescription
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class Tier 
{
    public string? Name { get; set; }
    public TierUnitType? Unit { get; set; }
    public ApplicationMethodType? ApplicationMethod { get; set; }
    public List<BalanceTierDetail>? BalanceTierDetails { get; set; }
    public List<LTVTierDetail>? LTVTierDetails { get; set; }
    public RateRange? RateRange { get; set; }
}

public class DepositRatesData
{
    public RateType RateType { get; set; }
    public List<RateDetail>? RateDetails { get; set; }
}
public class BalanceTierDetail
{
    public MoneyAmount? MinimumTierValue { get; set; }
    public MoneyAmount? MaximumTierValue { get; set; }
    public decimal? TierRate { get; set; }
}

public class LTVTierDetail
{
    public decimal? LTVStart { get; set; }
    public decimal? LTVEnd { get; set; }
    public decimal? TierRate { get; set; }
}

public class RateRange
{
    public decimal? MinimumRate { get; set; }
    public decimal? MaximumRate { get; set; }
    public string? AdditionalInformation { get; set; }
}

public class Condition
{
    public string? Field { get; set; }
    public string? Operator { get; set; }
    public string? Value { get; set; }
    public string? Description { get; set; }
}

public class RateDetail
{
    public RateCategoryType? RateCategory { get; set; }
    public decimal? AnnualRate { get; set; }
    public AnnualRateRange? AnnualRateRange { get; set; }
    public TierDetail? Tier { get; set; }
    public string? Term { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public CalculationMethodType? CalculationMethod { get; set; }
    public CalculationFrequencyType? CalculationFrequency { get; set; }
    public ApplicationFrequencyType? ApplicationFrequency { get; set; }
    public string? Notes { get; set; }
}

public class TierDetail
{
    public string? MinBalance { get; set; }
    public string? MaxBalance { get; set; }
    public string? Currency { get; set; }
}

[JsonConverter(typeof(ProductFinanceRateConverter))]
public abstract class ProductFinanceRate
{
    public FinanceRateType RateType { get; set; }
}

public abstract class FinanceRateBase : ProductFinanceRate
{
    public AnnualPercentageRate? AnnualPercentageRate { get; set; }

    public List<Tier>? Tiers { get; set; }

    public List<Condition>? Conditions { get; set; }

    public string? Notes { get; set; }

    public List<AdditionalInformationItem>? AdditionalInformation { get; set; }
}

public abstract class FixedRateBase : FinanceRateBase
{
    public string? Description { get; set; }

    public decimal? Rate { get; set; }

    public DateTime? FixedRateEndDate { get; set; }

    public FRCalculationFrequencyType? CalculationFrequency { get; set; }

    public FRApplicationFrequencyType? ApplicationFrequency { get; set; }
}

public abstract class VariableRateBase : FinanceRateBase
{
    public string? Description { get; set; }

    public decimal? Rate { get; set; }

    public string? BenchMark { get; set; }

    public decimal? BenchMarkRate { get; set; }

    public decimal? Margin { get; set; }

    public string? RateReviewFrequency { get; set; }

    public DateTime? RateReviewNextDate { get; set; }

    public FRCalculationFrequencyType? CalculationFrequency { get; set; }

    public FRApplicationFrequencyType? ApplicationFrequency { get; set; }
}

public class FixedInterest : FixedRateBase
{
    public InterestCalculationMethodType? InterestCalculationMethod { get; set; }
}

public class FixedProfit : FixedRateBase
{
    public ProfitCalculationMethodType? ProfitCalculationMethod { get; set; }
}

public class VariableInterest : VariableRateBase
{
    public InterestCalculationMethodType? InterestCalculationMethod { get; set; }
}

public class VariableProfit : VariableRateBase
{
    public ProfitCalculationMethodType? ProfitCalculationMethod { get; set; }
}

public class HybridInterest : FinanceRateBase
{
    public FixedRateInterest? FixedRate { get; set; }

    public VariableRateInterest? VariableRate { get; set; }
}

public class HybridProfit : FinanceRateBase
{
    public FixedRateProfit? FixedRate { get; set; }

    public VariableRateProfit? VariableRate { get; set; }
}

public class FixedRateInterest : FixedRateBase
{
    public InterestCalculationMethodType? InterestCalculationMethod { get; set; }

    public string? FixedRateEnd { get; set; }
}

public class VariableRateInterest : VariableRateBase
{
    public InterestCalculationMethodType? InterestCalculationMethod { get; set; }

    public string? VariableTerm { get; set; }
}

public class FixedRateProfit : FixedRateBase
{
    public ProfitCalculationMethodType? ProfitCalculationMethod { get; set; }

    public string? FixedRateEnd { get; set; }
}

public class VariableRateProfit : VariableRateBase
{
    public ProfitCalculationMethodType? ProfitCalculationMethod { get; set; }

    public string? VariableTerm { get; set; }
}

[JsonConverter(typeof(InterestRateOptionBaseConverter))]
public abstract class InterestRateOptionBase
{
    public FinanceRateType? RateType { get; set; }
}

public class InterestRateOptions : ProductFinanceRate
{
    public List<InterestRateOptionBase>? RateOptions { get; set; }
}

public class FixedInterestRateOption : InterestRateOptionBase
{
    public string? Description { get; set; }

    public decimal? Rate { get; set; }

    public DateTime? FixedRateEndDate { get; set; }

    public FRCalculationFrequencyType? CalculationFrequency { get; set; }

    public FRApplicationFrequencyType? ApplicationFrequency { get; set; }

    public InterestCalculationMethodType? InterestCalculationMethod { get; set; }

    public AnnualPercentageRate? AnnualPercentageRate { get; set; }

    public List<Tier>? Tiers { get; set; }

    public List<Condition>? Conditions { get; set; }

    public string? Notes { get; set; }

    public List<AdditionalInformationItem>? AdditionalInformation { get; set; }

    public List<IntroductoryPeriodOptions>? IntroductoryPeriodOptions { get; set; }
}

public class VariableInterestRateOption : InterestRateOptionBase
{
    public string? Description { get; set; }

    public decimal? Rate { get; set; }

    public string? BenchMark { get; set; }

    public decimal? BenchMarkRate { get; set; }

    public decimal? Margin { get; set; }

    public string? RateReviewFrequency { get; set; }

    public DateTime? RateReviewNextDate { get; set; }

    public FRCalculationFrequencyType? CalculationFrequency { get; set; }

    public FRApplicationFrequencyType? ApplicationFrequency { get; set; }

    public InterestCalculationMethodType? InterestCalculationMethod { get; set; }

    public AnnualPercentageRate? AnnualPercentageRate { get; set; }

    public List<Tier>? Tiers { get; set; }

    public List<Condition>? Conditions { get; set; }

    public string? Notes { get; set; }

    public List<AdditionalInformationItem>? AdditionalInformation { get; set; }
}

public class HybridInterestRateOption : InterestRateOptionBase
{
    public AnnualPercentageRate? AnnualPercentageRate { get; set; }

    public List<Tier>? Tiers { get; set; }

    public List<Condition>? Conditions { get; set; }

    public string? Notes { get; set; }

    public List<AdditionalInformationItem>? AdditionalInformation { get; set; }

    public HybridIntroductoryFixedInterest? FixedRate { get; set; }

    public VariableRateInterestOption? VariableRate { get; set; }
}

public class HybridIntroductoryFixedInterest
{
    public string? Description { get; set; }

    public decimal? Rate { get; set; }

    public DateTime? FixedRateEndDate { get; set; }

    public FRCalculationFrequencyType? CalculationFrequency { get; set; }

    public FRApplicationFrequencyType? ApplicationFrequency { get; set; }

    public InterestCalculationMethodType? InterestCalculationMethod { get; set; }

    public AnnualPercentageRate? AnnualPercentageRate { get; set; }

    public List<Tier>? Tiers { get; set; }

    public List<Condition>? Conditions { get; set; }

    public string? Notes { get; set; }

    public List<AdditionalInformationItem>? AdditionalInformation { get; set; }

    public string? FixedRateEnd { get; set; }

    public List<IntroductoryPeriodOptions>? IntroductoryPeriodOptions { get; set; }
}

public class VariableRateInterestOption
{
    public string? Description { get; set; }

    public decimal? Rate { get; set; }

    public string? BenchMark { get; set; }

    public decimal? BenchMarkRate { get; set; }

    public decimal? Margin { get; set; }

    public string? RateReviewFrequency { get; set; }

    public DateTime? RateReviewNextDate { get; set; }

    public FRCalculationFrequencyType? CalculationFrequency { get; set; }

    public FRApplicationFrequencyType? ApplicationFrequency { get; set; }

    public InterestCalculationMethodType? InterestCalculationMethod { get; set; }

    public AnnualPercentageRate? AnnualPercentageRate { get; set; }

    public List<Tier>? Tiers { get; set; }

    public List<Condition>? Conditions { get; set; }

    public string? Notes { get; set; }

    public List<AdditionalInformationItem>? AdditionalInformation { get; set; }

    public string? VariableTerm { get; set; }
}

[JsonConverter(typeof(ProfitRateOptionBaseConverter))]
public abstract class ProfitRateOptionBase
{
    public FinanceRateType? RateType { get; set; }
}

public class ProfitRateOptions : ProductFinanceRate
{
    public List<ProfitRateOptionBase>? RateOptions { get; set; }
}

public class FixedProfitRateOption : ProfitRateOptionBase
{
    public string? Description { get; set; }

    public decimal? Rate { get; set; }

    public DateTime? FixedRateEndDate { get; set; }

    public FRCalculationFrequencyType? CalculationFrequency { get; set; }

    public FRApplicationFrequencyType? ApplicationFrequency { get; set; }

    public ProfitCalculationMethodType? ProfitCalculationMethod { get; set; }

    public AnnualPercentageRate? AnnualPercentageRate { get; set; }

    public List<Tier>? Tiers { get; set; }

    public List<Condition>? Conditions { get; set; }

    public string? Notes { get; set; }

    public List<AdditionalInformationItem>? AdditionalInformation { get; set; }

    public List<IntroductoryPeriodOptions>? IntroductoryPeriodOptions { get; set; }
}

public class VariableProfitRateOption : ProfitRateOptionBase
{
    public string? Description { get; set; }

    public decimal? Rate { get; set; }

    public string? BenchMark { get; set; }

    public decimal? BenchMarkRate { get; set; }

    public decimal? Margin { get; set; }

    public string? RateReviewFrequency { get; set; }

    public DateTime? RateReviewNextDate { get; set; }

    public FRCalculationFrequencyType? CalculationFrequency { get; set; }

    public FRApplicationFrequencyType? ApplicationFrequency { get; set; }

    public ProfitCalculationMethodType? ProfitCalculationMethod { get; set; }

    public AnnualPercentageRate? AnnualPercentageRate { get; set; }

    public List<Tier>? Tiers { get; set; }

    public List<Condition>? Conditions { get; set; }

    public string? Notes { get; set; }

    public List<AdditionalInformationItem>? AdditionalInformation { get; set; }
}

public class HybridProfitRateOption : ProfitRateOptionBase
{
    public AnnualPercentageRate? AnnualPercentageRate { get; set; }

    public List<Tier>? Tiers { get; set; }

    public List<Condition>? Conditions { get; set; }

    public string? Notes { get; set; }

    public List<AdditionalInformationItem>? AdditionalInformation { get; set; }

    public HybridIntroductoryFixedProfit? FixedRate { get; set; }

    public VariableRateProfitOption? VariableRate { get; set; }
}

public class HybridIntroductoryFixedProfit
{
    public string? Description { get; set; }

    public decimal? Rate { get; set; }

    public DateTime? FixedRateEndDate { get; set; }

    public FRCalculationFrequencyType? CalculationFrequency { get; set; }

    public FRApplicationFrequencyType? ApplicationFrequency { get; set; }

    public ProfitCalculationMethodType? ProfitCalculationMethod { get; set; }

    public AnnualPercentageRate? AnnualPercentageRate { get; set; }

    public List<Tier>? Tiers { get; set; }

    public List<Condition>? Conditions { get; set; }

    public string? Notes { get; set; }

    public List<AdditionalInformationItem>? AdditionalInformation { get; set; }

    public string? FixedRateEnd { get; set; }

    public List<IntroductoryPeriodOptions>? IntroductoryPeriodOptions { get; set; }
}

public class VariableRateProfitOption
{
    public string? Description { get; set; }

    public decimal? Rate { get; set; }

    public string? BenchMark { get; set; }

    public decimal? BenchMarkRate { get; set; }

    public decimal? Margin { get; set; }

    public string? RateReviewFrequency { get; set; }

    public DateTime? RateReviewNextDate { get; set; }

    public FRCalculationFrequencyType? CalculationFrequency { get; set; }

    public FRApplicationFrequencyType? ApplicationFrequency { get; set; }

    public ProfitCalculationMethodType? ProfitCalculationMethod { get; set; }

    public AnnualPercentageRate? AnnualPercentageRate { get; set; }

    public List<Tier>? Tiers { get; set; }

    public List<Condition>? Conditions { get; set; }

    public string? Notes { get; set; }

    public List<AdditionalInformationItem>? AdditionalInformation { get; set; }

    public string? VariableTerm { get; set; }
}

public class IntroductoryPeriodOptions
{
    public string? Period { get; set; }

    public IndicativeRate? IndicativeRate { get; set; }
}

public class IndicativeRate
{
    public decimal? StartingFrom { get; set; }

    public decimal? UpTo { get; set; }
}


public class AnnualPercentageRate
{
    public string? StartingFrom { get; set; }

    public decimal? UpTo { get; set; }

    public string? AdditionalInformation { get; set; }
}

public class AnnualRateRange
{
    public decimal MinRate { get; set; }
    public decimal MaxRate { get; set; }
}
public class Tenor
{
    public string? MinimumTenor { get; set; }
    public string? MaximumTenor { get; set; }
    public string? Condition { get; set; }
}
public class AssetBacked
{
    public AssetBackedType? Type { get; set; }
    public AssetType? AssetType { get; set; }
    public string? Description { get; set; }
    public List<Valuation>? Valuation { get; set; }
    public object? SupplementaryInformation { get; set; }
    public OwnershipTransfer? OwnershipTransfer { get; set; }
}
public class RewardsBenefits
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public RewardBenefitType? Type { get; set; }
    public List<string>? RewardBasis { get; set; }
    public MoneyAmount? Balance { get; set; }
    public FrequencyPaidType? FrequencyPaid { get; set; }
    public string? PointsType { get; set; }
    public DateTime? ExpiryDate { get; set; }

}

public class CashbackBalance
{
   
    [Required]
    [RegularExpression(@"^\d{1,13}$|^\d{1,13}\.\d{1,5}$")]
    public string Amount { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^[A-Z]{3}$")]
    public string Currency { get; set; } = string.Empty;
}

public class Valuation
{
    public DateTime? Date { get; set; }

    public MoneyAmount? Amount { get; set; }
}

public class SupplementaryInformation
{
    public string? AdditionalData { get; set; }
}
public class MoneyAmount
{
    [RegularExpression(@"^\d{1,13}$|^\d{1,13}\.\d{1,5}$")]
    public string Amount{ get; set; } = string.Empty;

    [RegularExpression(@"^[A-Z]{3}$")]
    public string Currency { get; set; } = string.Empty;
}
public class BuyoutSchedule
{
    public PaymentFrequencyType? Frequency { get; set; }

    public MoneyAmount BuyoutAmount { get; set; } = new();
}

public class SaleAgreement
{
    public bool Required { get; set; }

    public SaleAgreementExecutionType? Execution { get; set; }
    public MoneyAmount? Price { get; set; }
}

public class OwnershipTransfer
{
    public DateTime? TransferOfOwnershipDate { get; set; }

    public OwnershipTransferType Type { get; set; }

    public OwnershipTransferMethodType? Method { get; set; }

    public MoneyAmount? TokenPurchaseAmount { get; set; }

    public BuyoutSchedule? BuyoutSchedule { get; set; }

    public SaleAgreement? SaleAgreement { get; set; }

    public List<TransferConditionType>? TransferConditions { get; set; }
}
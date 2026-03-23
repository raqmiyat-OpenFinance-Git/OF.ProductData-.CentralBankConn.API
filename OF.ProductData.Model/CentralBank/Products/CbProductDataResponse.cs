using OF.ProductData.Model.CentralBank.CreateLead;
using System.Text.Json.Serialization;

namespace OF.ProductData.Model.CentralBank.Products;
public class CbProductDataResponse
{
    public List<LFIData>? Data { get; set; }
}
public class LFIData
{
    public string LFIId { get; set; }
    public string LFIBrandId { get; set; }
    public List<ProductWrapper>? Products { get; set; }
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
    public ShariaStructure ShariaStructure { get; set; }
    public string? AlternativeBrandName { get; set; }
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
    public DepositRates ? depositRates { get; set; }
    public FinanceInterestRate? FinanceInterestRate { get; set; }
    public Tenor? Tenor { get; set; }
    public AssetBacked? AssetBacked { get; set; }
    public RewardsBenefits? RewardsBenefits { get; set; }
    public string?  DenominationCurrency { get; set; }


    //public ProfitSharingRateData? ProfitSharingRate { get; set; }
    //public FinanceProfitRateData? FinanceProfitRate { get; set; }
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

public class AdditionalInformation : TypeDescription { }
public class NameDescription
{

    public string? Name { get; set; }


    public string? Description { get; set; }
}

public class Tier 
{

    public string? Name { get; set; }
    public string? Unit { get; set; }
    public string? ApplicationMethod { get; set; }
    public List<BalanceTierDetail>? BalanceTierDetails { get; set; }
    public List<LTVTierDetail>? LTVTierDetails { get; set; }
    public RateRange? RateRange { get; set; }
}

public class DepositRates
{
    public RateType RateType { get; set; }
    public List<RateDetailsRate>? RateDetails { get; set; }
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


public class RateDetailsRate
{
    public RateCategory RateCategory { get; set; }

    public decimal? AnnualRate { get; set; }

    public AnnualRateRange? AnnualRateRange { get; set; }

    public Tiers? Tier { get; set; }

    public string Currency { get; set; } = string.Empty;
    public string? Term { get; set; }

    public DateTime? EffectiveDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public CalculationMethod CalculationMethod { get; set; }

    public Frequency CalculationFrequency { get; set; }

    public Frequency ApplicationFrequency { get; set; }

    public string? Notes { get; set; }
}

public class FinanceInterestRate
{
    public string? Description { get; set; }
    public decimal? Rate { get; set; }
    public string? BenchMark { get; set; }
    public decimal? BenchMarkRate { get; set; }
    public decimal? Margin { get; set; }
    public string? RateReviewFrequency { get; set; }
    public DateTime? RateReviewNextDate { get; set; }
    public string? CalculationFrequency { get; set; }
    public string? ApplicationFrequency { get; set; }
    public string? InterestCalculationMethod { get; set; }
    public AnnualPercentageRate? AnnualPercentageRate { get; set; }
    public List<Tier>? Tiers { get; set; }
    public List<Condition>? Conditions { get; set; }
    public string? Notes { get; set; }
    public List<AdditionalInformation>? AdditionalInformation { get; set; }
    public string? RateType { get; set; }

}
public class AnnualPercentageRate
{
    public decimal? StartingFrom { get; set; }

    public decimal? UpTo { get; set; }

    public string? AdditionalInformation { get; set; }
}
public class RateTiers
{
    public List<RateTier>? TierList { get; set; }
}
public class RateConditions
{
    public string? Description { get; set; }
}

public class AdditionalRateInformation
{
    public string? Details { get; set; }
}

public class RateTier
{
    public decimal FromAmount { get; set; }

    public decimal ToAmount { get; set; }

    public decimal Rate { get; set; }
}

public class AnnualRateRange
{
    public decimal MinRate { get; set; }
    public decimal MaxRate { get; set; }
}
public class Tiers
{
    public string MinBalance { get; set; } = string.Empty;
    public string MaxBalance { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
}
public class Tenor
{
    [RegularExpression(@"^P(\d+Y)?(\d+M)?$",
        ErrorMessage = "Invalid ISO 8601 duration format (e.g. P1Y, P6M, P2Y6M)")]
    public string? MinimumTenor { get; set; }

    [RegularExpression(@"^P(\d+Y)?(\d+M)?$",
        ErrorMessage = "Invalid ISO 8601 duration format (e.g. P1Y, P6M, P2Y6M)")]
    public string? MaximumTenor { get; set; }

    public string? Condition { get; set; }
}
public class AssetBacked
{
    public AssetBackedType Type { get; set; }

    public AssetType AssetType { get; set; }

    public string Description { get; set; } = string.Empty;

    public Valuation? Valuation { get; set; }

    public SupplementaryInformation? SupplementaryInformation { get; set; }

    public OwnershipTransfer? OwnershipTransfer { get; set; }
}
public class RewardsBenefits
{

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
    [Required]
    public RewardType Type { get; set; } = RewardType.Cashback;

    [Required]
    public CashbackBalance Balance { get; set; } = new();

    public List<string>? RewardBasis { get; set; }


    public FrequencyPaid? FrequencyPaid { get; set; }
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
public enum RewardType
{
    Cashback
}

public enum FrequencyPaid
{
    Daily,
    Weekly,
    Monthly,
    Quarterly,
    HalfYearly,
    Annually,
    UponRequest,
    Other
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
    public string Amount { get; set; } = string.Empty;

    [RegularExpression(@"^[A-Z]{3}$")]
    public string Currency { get; set; } = string.Empty;
}
public class BuyoutSchedule
{
    public PaymentFrequency Frequency { get; set; }

    public MoneyAmount BuyoutAmount { get; set; } = new();
}

public class SaleAgreement
{
    public bool Required { get; set; }

    public SaleAgreementExecution Execution { get; set; }
}

public class OwnershipTransfer
{
    public DateTime? TransferOfOwnershipDate { get; set; }

    public OwnershipTransferType Type { get; set; }

    public OwnershipTransferMethod? Method { get; set; }

    public MoneyAmount? TokenPurchaseAmount { get; set; }

    public BuyoutSchedule? BuyoutSchedule { get; set; }

    public SaleAgreement? SaleAgreement { get; set; }

    public MoneyAmount? Price { get; set; }

    public List<TransferCondition>? TransferConditions { get; set; }
}

public enum ShariaStructure
{
    Ijara, ServiceIjara, Murabaha, Musharaka, Tawarruq
}
public enum RateType
{
    FixedInterest,
    FixedProfit,
    VariableInterest,
    VariableProfit
}

public enum RateCategory
{
    Applied,
    Standard,
    Bonus,
    Introductory,
    Other
}

public enum CalculationMethod
{
    DailyClosingBalance,
    MinimumMonthlyBalance,
    AverageDailyBalance,
    AverageMonthlyBalance,
    AccountOpeningBalance,
    NotApplicable,
    Other
}

public enum Frequency
{
    Daily,
    Monthly,
    Quarterly,
    HalfYearly,
    Annually,
    NotApplicable
}



public enum InterestCalculationMethod
{
    PrincipalBalance,
    OutstandingBalance,
    InitialDrawdownAmount
}


public enum AssetBackedType
{
    Collateral,
    OwnershipTransfer
}

public enum AssetType
{
    Property,
    SalaryAssignment,
    EndOfServiceGratuity,
    SalaryAndGratuityAssignment,
    FixedDepositLien,
    Vehicle,
    TakafulPolicy,
    Rahn,
    PostDatedCheque,
    Other
}

public enum OwnershipTransferType
{
    Gift,
    TokenPurchase,
    Gradual,
    SeparateSaleContract
}

public enum OwnershipTransferMethod
{
    EndOfLease,
    Buyouts
}

public enum PaymentFrequency
{
    Weekly,
    Fortnightly,
    Monthly,
    Quarterly,
    HalfYearly,
    Annual,
    Other
}

public enum SaleAgreementExecution
{
    AtLeaseCompletion,
    CustomerRequestPostLease
}

public enum TransferCondition
{
    AllLeasePaymentsCompleted
}

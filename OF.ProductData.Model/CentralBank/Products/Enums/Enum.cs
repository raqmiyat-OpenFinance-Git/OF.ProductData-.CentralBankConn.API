namespace OF.ProductData.Model.CentralBank;

public enum ProductCategory
{
    SavingsAccount,
    CurrentAccount,
    CreditCard,
    Finance,
    Mortgage,
}

public enum ShariaStructure
{
    Ijara,
    ServiceIjara,
    Murabaha,
    Musharaka,
    Tawarruq
}

public enum ResidenceStatusType
{
    UaeResident,
    ExpatriatesResident,
    GCCNational,
    NonUaeResident
}

public enum AdditionalInformationType
{
    Other
}

public enum EmploymentStatusType
{
    Salaried,
    NonSalaried,
    EligibleEmployer,
    SelfEmployed,
    Unemployed,
    MinimumSalary,
    EmployerEligibility,
    MinimumEmploymentTenure
}

public enum CustomerType
{
    Retail,
    SME,
    Corporate,
    New,
    Existing
}

public enum AccountOwnershipType
{
    Individual,
    Joint,
    Multi
}

public enum AgeType
{
    MinimumAge,
    MaximumAge
}

public enum FinancialRequirementType
{
    MinimumCreditScore,
    NoRecentDefaults,
    NoBankruptcyHistory,
    MaximumDelinquenciesAllowed,
    MinimumCreditHistoryLength,
    MinimumDisposableIncome,
    MinimumSalaryTransferRequirement,
    MaximumDebtToIncomeRatio,
    DebtBurdenRatio,
    Other
}

public enum AdditionalEligibilityType
{
    Student,
    UaeArmedForces,
    TimeInCountry,
    Other
}

public enum ChannelType
{
    Phone,
    Internet,
    MobileApp,
    Branch,
    RelationshipManager,
    Other
}

public enum CurrentAccountType
{
    Basic,
    Premium,
    Advanced,
    Other
}

public enum SavingsAccountType
{
    Savings,
    OnlineSaver,
    TermDeposit,
    Notice,
    Other
}

public enum CreditCardType
{
    Visa,
    Mastercard,
    AmericanExpress,
    Diners,
    Other
}

public enum FinanceType
{
    Finance,
    DebtSettlement,
    EducationFinance,
    TravelFinance,
    BoatFinance,
    AutoFinance,
    LineOfCredit,
    SalaryAdvance,
    HomeImprovement,
    MedicalLoan,
    Other
}

public enum DocumentType
{
    ApplicationForm,
    IDCard,
    Passport,
    ResidenceVisa,
    ProofofAddress,
    UtilityBill,
    SalaryCertificate,
    SalarySlip,
    TradeLicence,
    BusinessOwnership,
    Other,
    BankStatement,
    UndatedSecurityCheque,
    ProofofBusinessOwnership,
    MemorandumofAssociation
}

public enum FeatureTypeCurrentAccount
{
    IslamicBanking,
    IslamicFinance,
    DebitCard,
    MultipleCurrencies,
    DigitalWallet,
    ChequeBook,
    Cashback,
    ATMTransfers,
    FreeCashWithdrawal,
    FreeInternationalPayments,
    Contactless,
    CardlessATMWithdrawal,
    TellerTransactions,
    SMSNotifications,
    BillPayments,
    FundsTransfer,
    SalaryAdvance,
    WorldWideSupport,
    Other
}

public enum FeatureTypeSavingsAccount
{
    IslamicBanking,
    IslamicFinance,
    DebitCard,
    MultipleCurrencies,
    ATMCard,
    FreeATMAccess,
    ChequeBook,
    CardlessChequeDeposit,
    CounterTransactions,
    MoneyTransfer,
    NoLockIn,
    SMSNotifications,
    Other
}

public enum FeatureTypeCreditCard
{
    IslamicBanking,
    IslamicFinance,
    AdditionalCards,
    CoBrandedCard,
    BalanceTransfer,
    CashWithdrawal,
    InternationalPayments,
    FlexiblePaymentTerm,
    InstalmentPaymentPlans,
    InsuranceProtection,
    CreditShield,
    CardControl,
    PersonalisedCardDesign,
    Other
}

public enum FeatureTypeFinance
{
    IslamicBanking,
    IslamicFinance,
    QuickApproval,
    FreeDebitCard,
    FreeCreditCard,
    FlexibleRepaymentPeriods,
    BuyOut,
    TransferPersonalFinance,
    FinanceConsolidation,
    FinanceTopUp,
    RevolvingOverdraft,
    DefermentFacility,
    LifeInsurance,
    Guarantor,
    Other
}

public enum FeatureTypeMortgage
{
    IslamicBanking,
    IslamicFinance,
    OnlineApplication,
    PreApproval,
    LifeInsurance,
    PropertyInsurance,
    GracePeriod,
    Overpayments,
    Redraw,
    FreeCreditCard,
    FreePropertyValuation,
    BuyOut,
    TopUp,
    InstalmentDeferment,
    EquityRelease,
    NoDeveloperRestriction,
    Guarantor,
    Other
}

public enum ChargeType
{
    MonthlyFees,
    AnnualFees,
    BalanceFallBelow,
    Overdraft,
    DomesticTransaction,
    ForeignTransaction,
    ATMWithdrawal,
    ChequeBook,
    ReplacementChequeBook,
    ReturnCheque,
    CardReplacement,
    AccountStatement,
    LetterIssued,
    AccountClosure,
    CashAdvance,
    LatePayment,
    BalanceTransfer,
    OverLimit,
    SalesVoucherCopy,
    Processing,
    FinanceCancellation,
    PartialPayment,
    EarlySettlement,
    FinalSettlement,
    Other
}

public enum LimitTypeCurrentAccount
{
    MinimumDeposit,
    MinimumBalance,
    MaximumWithdrawal,
    MaximumCashATMWithdrawal,
    MaximumInternationalATMWithdrawal,
    RetailDomesticPurchase,
    RetailInternationalPurchase,
    TellerTransactions,
    Other
}

public enum LimitTypeSavingsAccount
{
    MinimumOpeningBalance,
    MaximumBalance,
    MaximiumWithdrawal,
    MaximumCashATMWithdrawal,
    MaximumInternationalATMWithdrawal,
    MaximumTellerTransactions,
    Other
}

public enum LimitTypeCreditCard
{
    MinimumCreditLimit,
    MaximumCreditLimit,
    MinimumSpend,
    MaximumCashATMWithdrawal,
    MaximumInternationalATMWithdrawal,
    Other
}

public enum LimitTypeFinance
{
    MaximumOverpayment,
    Other
}

public enum LimitTypeMortgage
{
    MaximumOverpayment,
    Other
}

public enum ApplicationFrequencyType
{
    Monthly,
    Daily
}

public enum CalculationFrequencyType
{
    Monthly,
    Daily,
    Quarterly,
    HalfYearly,
    Annually,
    NotApplicable
}

public enum CalculationMethodType
{
    DailyClosingBalance,
    MinimumMonthlyBalance,
    AverageDailyBalance,
    AverageMonthlyBalance,
    AccountOpeningBalance,
    NotApplicable,
    Other
}

public enum RateCategoryType
{
    Applied,
    Standard,
    Bonus,
    Introductory,
    Other
}

public enum RateType
{
    FixedInterest,
    FixedProfit,
    VariableInterest,
    VariableProfit
}
public enum FinanceRateType
{
    FixedInterest,
    FixedProfit,
    VariableInterest,
    VariableProfit,
    HybridInterest,
    HybridProfit,
    InterestRateOptions,
    ProfitRateOptions
}

public enum FRCalculationFrequencyType
{
    Monthly,
    Daily,
    Quarterly,
    Annually,
}

public enum FRApplicationFrequencyType
{
    Monthly,
    Daily,
    Quarterly,
    Annually,
}

public enum InterestCalculationMethodType
{
    PrincipalBalance,
    OutstandingBalance,
    InitialDrawdownAmount
}

public enum ProfitCalculationMethodType
{
    PrincipalBalance,
    OutstandingBalance,
    InitialDrawdownAmount
}

public enum TierUnitType
{
    Balance,
    LTV,
    Rate
}

public enum ApplicationMethodType
{
    PerTier,
    WholeBalance
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

public enum OwnershipTransferMethodType
{
    EndOfLease,
    Buyouts
}

public enum PaymentFrequencyType
{
    Weekly,
    Fortnightly,
    Monthly,
    Quarterly,
    HalfYearly,
    Annual,
    Other
}

public enum SaleAgreementExecutionType
{
    AtLeaseCompletion,
    CustomerRequestPostLease
}

public enum TransferConditionType
{
    AllLeasePaymentsCompleted,
    CustomerRequestRequired,
    SaleAgreementExecuted,
    AllBuyoutsCompleted,
    IndependentSaleContractSigned
}

public enum RewardBenefitType
{
    Cashback,
    Points,
    ComplimentaryServices,
    Discounts,
    Lifestyle,
    Protection,
    Other
}

public enum FrequencyPaidType
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
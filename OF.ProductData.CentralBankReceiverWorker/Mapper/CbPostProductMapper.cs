using Newtonsoft.Json;
using OF.ProductData.Model.CentralBank.Products;
using OF.ProductData.Model.EFModel.Products;
using EFTenor = OF.ProductData.Model.EFModel.Products.Tenor;
using EFAssetBacked = OF.ProductData.Model.EFModel.Products.AssetBacked;
using EFRewardsBenefits = OF.ProductData.Model.EFModel.Products.RewardsBenefits;

namespace OF.ServiceInitiation.CentralBankReceiverWorker.Mappers
{
    public static class CbPostProductMapper
    {
        public static List<EFProductResponse> MapCbPostProductResponsetToEF(CbProductResponseWrapper requestDto, long paymentRequestId)
        {
            var productList = new List<EFProductResponse>();

            if (requestDto?.centralBankProductResponse?.Data == null)
                return productList;

            
            foreach (var lfiData in requestDto.centralBankProductResponse.Data)
            {
                if (lfiData.Products == null) continue;

                foreach (var prod in lfiData.Products)
                {
                    var productData = new EFProductResponse
                    {
                        LFIId = lfiData.LFIId,
                        LFIBrandId = lfiData.LFIBrandId,
                        ProductId = prod.ProductId,
                        ProductName = prod.ProductName,
                        ProductCategory = prod.ProductCategory.ToString(),
                        Description = prod.Description,
                        EffectiveFromDateTime = prod.EffectiveFromDateTime,
                        EffectiveToDateTime = prod.EffectiveToDateTime,
                        LastUpdatedDateTime = prod.LastUpdatedDateTime,
                        IsShariaCompliant = prod.IsShariaCompliant,
                        ShariaInformation = prod.ShariaInformation,
                        IsSalaryTransferRequired = prod.IsSalaryTransferRequired,

                        // Flattened Links
                        ApplicationUri = prod.Links?.ApplicationUri,
                        ApplicationEmail = prod.Links?.ApplicationEmail,
                        ApplicationPhoneNumber = prod.Links?.ApplicationPhoneNumber,
                        KfsUri = prod.Links?.KfsUri,
                        TermsUri = prod.Links?.TermsUri,
                        OverviewUri = prod.Links?.OverviewUri,
                        FeesAndPricingUri = prod.Links?.FeesAndPricingUri,
                        ScheduleOfChargesUri = prod.Links?.ScheduleOfChargesUri,
                        EligibilityUri = prod.Links?.EligibilityUri,
                        CardImageUri = prod.Links?.CardImageUri,
                        ApplicationDescription = prod.Links?.ApplicationDescription,

                        // Eligibility
                        ChannelsType = prod.Channels?.FirstOrDefault()?.Type,
                        ChannelsDescription = prod.Channels?.FirstOrDefault()?.Description,
                        ResidenceStatusType = prod.Eligibility?.ResidenceStatus?.FirstOrDefault()?.Type.ToString(),
                        ResidenceStatusDescription = prod.Eligibility?.ResidenceStatus?.FirstOrDefault()?.Description,
                        EmploymentStatusType = prod.Eligibility?.EmploymentStatus?.FirstOrDefault()?.Type.ToString(),
                        EmploymentStatusDescription = prod.Eligibility?.EmploymentStatus?.FirstOrDefault()?.Description,
                        CustomerTypeType = prod.Eligibility?.CustomerType?.FirstOrDefault()?.Type.ToString(),
                        CustomerTypeDescription = prod.Eligibility?.CustomerType?.FirstOrDefault()?.Description,
                        AccountOwnershipType = prod.Eligibility?.AccountOwnership?.FirstOrDefault()?.Type.ToString(),
                        AccountOwnershipDescription = prod.Eligibility?.AccountOwnership?.FirstOrDefault()?.Description,
                        AgeEligibilityType = prod.Eligibility?.Age?.FirstOrDefault()?.Type.ToString(),
                        AgeEligibilityValue = prod.Eligibility?.Age?.FirstOrDefault()?.Value ?? 0,
                        AgeEligibilityDescription = prod.Eligibility?.Age?.FirstOrDefault()?.Description,
                        FinancialRequirementsType =  prod.Eligibility?.FinancialRequirements?.FirstOrDefault()?.Type.ToString(),
                        FinancialRequirementsDescription =  prod.Eligibility?.FinancialRequirements?.FirstOrDefault()?.Description?.ToString(),
                        FinancialRequirementsValue =  prod.Eligibility?.FinancialRequirements?.FirstOrDefault()?.Value?.ToString(),
                        FinancialRequirementsAmount =  prod.Eligibility?.FinancialRequirements?.FirstOrDefault()?.Amount?.AmountValue,
                        FinancialRequirementsCurrency =  prod.Eligibility?.FinancialRequirements?.FirstOrDefault()?.Amount?.Currency,

                        AdditionalEligibilityType = prod.Eligibility?.AdditionalEligibility?.FirstOrDefault()?.Type.ToString(),
                        AdditionalEligibilityDescription = prod.Eligibility?.AdditionalEligibility?.FirstOrDefault()?.Description,

                        // Nested product types
                        CurrentAccount = MapCurrentAccount(prod.Product?.CurrentAccount!, paymentRequestId),
                        SavingsAccount = MapSavingsAccount(prod.Product?.SavingsAccount!, paymentRequestId),
                        CreditCard = MapCreditCard(prod.Product?.CreditCard!, paymentRequestId),
                        Finance = MapFinanace(prod.Product?.Finance!, paymentRequestId),
                        Mortgage = MapMortgage(prod.Product?.Mortgage!, paymentRequestId),
                        DepositRates = MapDepositRates(prod.Product?.DepositRates!, paymentRequestId),
                        FinanceRates = MapFinanaceRate(prod.Product?.FinanceRates!, paymentRequestId),
                        Tenor = prod.Product?.Tenor?.Any() == true? MapTenor(prod.Product.Tenor.First(), paymentRequestId).ToList(): new List<EFTenor>(),
                        AssetBacked = prod.Product?.AssetBacked?.Any() == true? MapAssetBacked(prod.Product.AssetBacked.First(), paymentRequestId).ToList(): new List<EFAssetBacked>(),
                        RewardsBenefits = prod.Product?.RewardsBenefits?.Any() == true ? MapRewardsBenefits(prod.Product.RewardsBenefits.First(), paymentRequestId).ToList() : new List<EFRewardsBenefits>(),

                    };

                    productData.CreatedBy = "System";
                    productData.Status = "PROCESSED";
                    productData.CreatedOn = DateTime.UtcNow;
                    productData.ResponsePayload = JsonConvert.SerializeObject(requestDto.centralBankProductResponse.Data);
                    productData.RequestId = paymentRequestId;
                    productList.Add(productData);
                }
            }

            return productList;
        }

        // -------------------- Helper mapping methods --------------------

        public static ICollection<CurrentAccounts> MapCurrentAccount(CurrentAccountData src, long paymentRequestId)
        {
            if (src == null) return null;

            return new List<CurrentAccounts>
            {
                 new CurrentAccounts
                    {
                        RequestId = paymentRequestId,
                        Type = src.Type.ToString(),

                        IsOverdraftAvailable = src.IsOverdraftAvailable,
                        // Documentation
                        DocumentationType = src.Documentation?.FirstOrDefault()?.Type.ToString(),
                        DocumentationDescription = src.Documentation?.FirstOrDefault()?.Description,

                        // Features
                        FeaturesType = src.Features?.FirstOrDefault()?.Type.ToString(),
                        FeaturesDescription = src.Features?.FirstOrDefault()?.Description,

                        // Charges
                        ChargesType=src.Charges?.FirstOrDefault()?.Type.ToString(),
                        ChargesName=src.Charges?.FirstOrDefault()?.Name,
                        ChargesDescription=src.Charges?.FirstOrDefault()?.Description,
                        ChargeAmount = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.Amount?.AmountValue,
                        ChargeCurrency = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.Amount?.Currency,
                        ChargeRate = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.Rate,
                        ChargeApplicationFrequency = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.ApplicationFrequency,
                        ChargeInterestCalculationMethod = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.InterestCalculationMethod,
                        MaximumChargeAmount = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.MaximumChargeAmount?.AmountValue,
                        MaximumChargeCurrency = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.MaximumChargeAmount?.Currency,
                        ChargeBasis = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.Basis,
                        ConditionsField = src.Charges?.FirstOrDefault()?.Conditions?.FirstOrDefault()?.Field,
                        ConditionsOperator = src.Charges?.FirstOrDefault()?.Conditions?.FirstOrDefault()?.Operator,
                        ConditionsValue = src.Charges?.FirstOrDefault()?.Conditions?.FirstOrDefault()?.Value,
                        ConditionsDescription = src.Charges?.FirstOrDefault()?.Conditions?.FirstOrDefault()?.Description,

                        Justification = src.Charges?.FirstOrDefault()?.Justification,
                        Frequency = src.Charges?.FirstOrDefault()?.Frequency,
                        DonatedToCharity = src.Charges?.FirstOrDefault()?.DonatedToCharity,
                        Notes = src.Charges?.FirstOrDefault()?.Notes,
                        SupplementaryInformation = src.Charges?.FirstOrDefault()?.SupplementaryInformation?.ToString(),

                        // Limits
                        LimitsType = src.Limits?.FirstOrDefault()?.Type.ToString(),
                        LimitsDescription = src.Limits?.FirstOrDefault()?.Description,
                        LimitsValue = src.Limits?.FirstOrDefault()?.Value,
                        LimitsAmount = src.Limits?.FirstOrDefault()?.Amount?.AmountValue,
                        LimitsCurrency = src.Limits?.FirstOrDefault()?.Amount?.Currency,

                 }
            };
        }
        public static ICollection<SavingsAccount> MapSavingsAccount(SavingsAccountData src, long paymentRequestId)
        {
            if (src == null) return null;

            return new List<SavingsAccount>
            {
               new SavingsAccount
               {
                    RequestId = paymentRequestId,
                    Type = src.Type.ToString(),

                    MinimumBalance=Convert.ToDecimal(src.MinimumBalance!.AmountValue),
                    Currency=src.MinimumBalance.Currency,

                    DocumentationType = src.Documentation?.FirstOrDefault()?.Type.ToString(),
                    DocumentationDescription = src.Documentation?.FirstOrDefault()?.Description,

                    // Features
                    FeaturesType = src.Features?.FirstOrDefault()?.Type.ToString(),
                    FeaturesDescription = src.Features?.FirstOrDefault()?.Description,

                    // Charges
                    ChargesType=src.Charges?.FirstOrDefault()?.Type.ToString(),
                    ChargesName=src.Charges?.FirstOrDefault()?.Name,
                    ChargesDescription=src.Charges?.FirstOrDefault()?.Description,
                    ChargeAmount = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.Amount?.AmountValue,
                    ChargeCurrency = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.Amount?.Currency,
                    ChargeRate = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.Rate,
                    ChargeApplicationFrequency = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.ApplicationFrequency,
                    ChargeInterestCalculationMethod = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.InterestCalculationMethod,
                    MaximumChargeAmount = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.MaximumChargeAmount?.AmountValue,
                    MaximumChargeAmountCurrency = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.MaximumChargeAmount?.Currency,
                    ChargeBasis = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.Basis,
                    ConditionsField = src.Charges?.FirstOrDefault()?.Conditions?.FirstOrDefault()?.Field,
                    ConditionsOperator = src.Charges?.FirstOrDefault()?.Conditions?.FirstOrDefault()?.Operator,
                    ConditionsValue = src.Charges?.FirstOrDefault()?.Conditions?.FirstOrDefault()?.Value,
                    ConditionsDescription = src.Charges?.FirstOrDefault()?.Conditions?.FirstOrDefault()?.Description,

                    Justification = src.Charges?.FirstOrDefault()?.Justification,
                    Frequency = src.Charges?.FirstOrDefault()?.Frequency,
                    DonatedToCharity = src.Charges?.FirstOrDefault()?.DonatedToCharity,
                    Notes = src.Charges?.FirstOrDefault()?.Notes,
                    SupplementaryInformation = src.Charges?.FirstOrDefault()?.SupplementaryInformation?.ToString(),


                    // Limits
                    LimitsType = src.Limits?.FirstOrDefault()?.Type.ToString(),
                    LimitsDescription = src.Limits?.FirstOrDefault()?.Description,
                    LimitsValue = src.Limits?.FirstOrDefault()?.Value,
                    LimitsAmount = src.Limits?.FirstOrDefault()?.Amount?.AmountValue,
                    LimitsCurrency = src.Limits?.FirstOrDefault()?.Amount?.Currency,
               }
            };
        }

        public static ICollection<CreditCard> MapCreditCard(CreditCardData src, long paymentRequestId)
        {
            if (src == null) return null;

            return new List<CreditCard>
            {
                new CreditCard{
                    RequestId = paymentRequestId,
                    Type = src.Type.ToString(),
                    DocumentationType = src.Documentation?.FirstOrDefault()?.Type.ToString(),
                    DocumentationDescription = src.Documentation?.FirstOrDefault()?.Description,
                    FeaturesType = src.Features?.FirstOrDefault()?.Type.ToString(),
                    FeaturesDescription = src.Features?.FirstOrDefault()?.Description,

                    // Charges
                    ChargesType=src.Charges?.FirstOrDefault()?.Type.ToString(),
                    ChargesName=src.Charges?.FirstOrDefault()?.Name,
                    ChargesDescription=src.Charges?.FirstOrDefault()?.Description,
                    ChargeAmount = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.Amount?.AmountValue,
                    ChargeCurrency = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.Amount?.Currency,
                    ChargeRate = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.Rate,
                    ChargeApplicationFrequency = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.ApplicationFrequency,
                    ChargeInterestCalculationMethod = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.InterestCalculationMethod,
                    MaximumChargeAmount = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.MaximumChargeAmount?.AmountValue,
                    MaximumChargeAmountCurrency = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.MaximumChargeAmount?.Currency,
                    ChargeBasis = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.Basis,
                    ConditionsField = src.Charges?.FirstOrDefault()?.Conditions?.FirstOrDefault()?.Field,
                    ConditionsOperator = src.Charges?.FirstOrDefault()?.Conditions?.FirstOrDefault()?.Operator,
                    ConditionsValue = src.Charges?.FirstOrDefault()?.Conditions?.FirstOrDefault()?.Value,
                    ConditionsDescription = src.Charges?.FirstOrDefault()?.Conditions?.FirstOrDefault()?.Description,

                    Justification = src.Charges?.FirstOrDefault()?.Justification,
                    Frequency = src.Charges?.FirstOrDefault()?.Frequency,
                    DonatedToCharity = src.Charges?.FirstOrDefault()?.DonatedToCharity,
                    Notes = src.Charges?.FirstOrDefault()?.Notes,
                    SupplementaryInformation = src.Charges?.FirstOrDefault()?.SupplementaryInformation?.ToString(),

                    LimitsType = src.Limits?.FirstOrDefault()?.Type.ToString(),
                    LimitsDescription = src.Limits?.FirstOrDefault()?.Description,
                    LimitsValue = src.Limits ?.FirstOrDefault() ?.Value,
                    LimitsAmount = src.Limits?.FirstOrDefault()?.Amount?.AmountValue,
                    LimitsCurrency = src.Limits?.FirstOrDefault()?.Amount?.Currency,
                }
            };
        }

        public static ICollection<Finance> MapFinanace(FinanceData src, long paymentRequestId)
        {
            if (src == null) return null;
            return new List<Finance>
            {
                new Finance
                {
                        RequestId = paymentRequestId,
                        Type = src.Type.ToString(),
                        MinimumFinanceAmount=src.MinimumFinanceAmount?.AmountValue,
                        MinimumFinanceCurrency=src.MinimumFinanceAmount?.Currency,
                        MaximumFinanceAmount=src.MaximumFinanceAmount?.AmountValue,
                        MaximumFinanceCurrency=src.MaximumFinanceAmount?.Currency,
                        // Documentation
                        DocumentationType = src.Documentation?.FirstOrDefault()?.Type.ToString(),
                        DocumentationDescription = src.Documentation?.FirstOrDefault()?.Description,

                        // Features
                        FeaturesType = src.Features?.FirstOrDefault()?.Type.ToString(),
                        FeaturesDescription = src.Features?.FirstOrDefault()?.Description,

                        // Charges
                        ChargesType=src.Charges?.FirstOrDefault()?.Type.ToString(),
                        ChargesName=src.Charges?.FirstOrDefault()?.Name,
                        ChargesDescription=src.Charges?.FirstOrDefault()?.Description,
                        ChargeAmount = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.Amount?.AmountValue,
                        ChargeCurrency = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.Amount?.Currency,
                        ChargeRate = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.Rate,
                        ChargeApplicationFrequency = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.ApplicationFrequency,
                        ChargeInterestCalculationMethod = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.InterestCalculationMethod,
                        MaximumChargeAmount = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.MaximumChargeAmount?.AmountValue,
                        MaximumChargeCurrency = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.MaximumChargeAmount?.Currency,
                        ChargeBasis = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.Basis,
                        ConditionsField = src.Charges?.FirstOrDefault()?.Conditions?.FirstOrDefault()?.Field,
                        ConditionsOperator = src.Charges?.FirstOrDefault()?.Conditions?.FirstOrDefault()?.Operator,
                        ConditionsValue = src.Charges?.FirstOrDefault()?.Conditions?.FirstOrDefault()?.Value,
                        ConditionsDescription = src.Charges?.FirstOrDefault()?.Conditions?.FirstOrDefault()?.Description,

                        Justification = src.Charges?.FirstOrDefault()?.Justification,
                        Frequency = src.Charges?.FirstOrDefault()?.Frequency,
                        DonatedToCharity = src.Charges?.FirstOrDefault()?.DonatedToCharity,
                        Notes = src.Charges?.FirstOrDefault()?.Notes,
                        SupplementaryInformation = src.Charges?.FirstOrDefault()?.SupplementaryInformation?.ToString(),

                        // Limits
                        LimitsType = src.Limits?.FirstOrDefault()?.Type.ToString(),
                        LimitsDescription = src.Limits?.FirstOrDefault()?.Description,
                        LimitsValue = src.Limits?.FirstOrDefault()?.Value,
                        LimitsAmount = src.Limits?.FirstOrDefault()?.Amount?.AmountValue,
                        LimitsCurrency = src.Limits?.FirstOrDefault()?.Amount?.Currency,

                        //Additional Information
                        AdditionalInfoType = src.AdditionalInformation?.FirstOrDefault()?.Type.ToString(),
                        AdditionalInfoDescription = src.AdditionalInformation?.FirstOrDefault()?.Description,
                }
            };
        }

        public static ICollection<Mortgage> MapMortgage(MortgageData src, long paymentRequestId)
        {
            if (src == null) return null;
            return new List<Mortgage>
            {
                new Mortgage
                {
                    RequestId = paymentRequestId,
                    MinimumFinanceAmount=src.MinimumFinanceAmount?.AmountValue,
                    MinimumFinanceCurrency=src.MinimumFinanceAmount?.Currency,
                    MaximumFinanceAmount=src.MaximumFinanceAmount?.AmountValue,
                    MaximumFinanceCurrency=src.MaximumFinanceAmount?.Currency,
                    DownPaymentCustomerCategory=src.DownPayment?.FirstOrDefault()?.CustomerCategory,
                    DownPaymentMinimumPercent=src.DownPayment?.FirstOrDefault()?.MinimumPercent,
                    DownPaymentBasis=src.DownPayment?.FirstOrDefault()?.Basis,
                    DocumentationType = src.Documentation?.FirstOrDefault()?.Type.ToString(),
                    DocumentationDescription = src.Documentation?.FirstOrDefault()?.Description,
                    FeaturesType = src.Features?.FirstOrDefault()?.Type.ToString(),
                    FeaturesDescription = src.Features?.FirstOrDefault()?.Description,
                    // Charges
                    ChargesType=src.Charges?.FirstOrDefault()?.Type.ToString(),
                    ChargesName=src.Charges?.FirstOrDefault()?.Name,
                    ChargesDescription=src.Charges?.FirstOrDefault()?.Description,
                    ChargeAmount = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.Amount?.AmountValue,
                    ChargeCurrency = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.Amount?.Currency,
                    ChargeRate = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.Rate,
                    ChargeApplicationFrequency = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.ApplicationFrequency,
                    ChargeInterestCalculationMethod = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.InterestCalculationMethod,
                    MaximumChargeAmount = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.MaximumChargeAmount?.AmountValue,
                    MaximumChargeCurrency = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.MaximumChargeAmount?.Currency,
                    ChargeBasis = src.Charges?.FirstOrDefault()?.Charge?.FirstOrDefault()?.Basis,
                    ConditionsField = src.Charges?.FirstOrDefault()?.Conditions?.FirstOrDefault()?.Field,
                    ConditionsOperator = src.Charges?.FirstOrDefault()?.Conditions?.FirstOrDefault()?.Operator,
                    ConditionsValue = src.Charges?.FirstOrDefault()?.Conditions?.FirstOrDefault()?.Value,
                    ConditionsDescription = src.Charges?.FirstOrDefault()?.Conditions?.FirstOrDefault()?.Description,
                    Justification = src.Charges?.FirstOrDefault()?.Justification,
                    Frequency = src.Charges?.FirstOrDefault()?.Frequency,
                    DonatedToCharity = src.Charges?.FirstOrDefault()?.DonatedToCharity,
                    Notes = src.Charges?.FirstOrDefault()?.Notes,
                    SupplementaryInformation = src.Charges?.FirstOrDefault()?.SupplementaryInformation?.ToString(),
                    LimitsType = src.Limits?.FirstOrDefault()?.Type.ToString(),
                    LimitsDescription = src.Limits?.FirstOrDefault()?.Description,
                    LimitsValue = src.Limits?.FirstOrDefault()?.Value,
                    LimitsAmount = src.Limits?.FirstOrDefault()?.Amount?.AmountValue,
                    LimitsCurrency = src.Limits?.FirstOrDefault()?.Amount?.Currency,
                    LimitsPercentage=src.Limits?.FirstOrDefault()?.Percentage
                }
            };
        }
        public static ICollection<DepositRates> MapDepositRates(DepositRatesData src, long paymentRequestId)
        {
            if (src == null) return null;
            var rate = src.RateDetails?.FirstOrDefault();
            var tier = src.RateDetails?.FirstOrDefault()?.Tier;

            return new List<DepositRates>
            {
              new DepositRates
              {
                    DepositId = Guid.NewGuid(),
                    RequestId = paymentRequestId,
                    RateType = src.RateType.ToString(),
                    RateCategory = rate?.RateCategory.ToString(),
                    AnnualRate = rate?.AnnualRate,
                    AnnualMinRate=rate?.AnnualRateRange?.MinRate,
                    AnnualMaxRate=rate?.AnnualRateRange?.MaxRate,
                    TierMinBalance = tier?.MinBalance,
                    TierMaxBalance = tier?.MaxBalance,
                    TierCurrency = tier?.Currency,
                    Term = rate?.Term,
                    EffectiveDate = rate?.EffectiveDate,
                    ExpiryDate = rate?.ExpiryDate,
                    CalculationMethod = rate?.CalculationMethod.ToString(),
                    CalculationFrequency = rate?.CalculationFrequency.ToString(),
                    ApplicationFrequency = rate?.ApplicationFrequency.ToString(),
                    Notes = rate?.Notes
                }
            };
        }
        public static ICollection<FinanceRates> MapFinanaceRate(FinanceRatesData src, long paymentRequestId)
        {
            var rateOption = src?.RateOption?.FirstOrDefault();
            var tier = rateOption?.Tiers?.FirstOrDefault();
            var balanceTier = tier?.BalanceTierDetails?.FirstOrDefault();
            var LvtTier = tier?.LTVTierDetails?.FirstOrDefault();

            var FixedTier = rateOption?.FixedRate?.Tiers?.FirstOrDefault();
            var FixedbalanceTier = FixedTier?.BalanceTierDetails?.FirstOrDefault();
            var FixedLvtTier = FixedTier?.LTVTierDetails?.FirstOrDefault();
            var FixedIntroductoryPeriodOptions = rateOption?.FixedRate?.IntroductoryPeriodOptions?.FirstOrDefault();

            var VariableTiers = rateOption?.VariableRate?.Tiers?.FirstOrDefault();
            var VariablebalanceTier = VariableTiers?.BalanceTierDetails?.FirstOrDefault();
            var VariableLvtTier = VariableTiers?.LTVTierDetails?.FirstOrDefault();

            var Conditions = rateOption?.Conditions?.FirstOrDefault();
            var Additionalinfo = rateOption?.AdditionalInformation?.FirstOrDefault();

            if (src == null) return null;
            var finance = new FinanceRates
            {

                RequestId = paymentRequestId,
                FinanceRateType = src?.RateType,
                RateOption_RateType = rateOption?.RateType.ToString(),
                APR_StartingFrom = rateOption?.AnnualPercentageRate?.StartingFrom,
                APR_UpTo = rateOption?.AnnualPercentageRate?.UpTo,
                APR_AdditionalInformation = rateOption?.AnnualPercentageRate?.AdditionalInformation,
                Tier_Name = tier?.Name,
                Tier_Unit = tier?.Unit?.ToString(),
                Tier_ApplicationMethod = tier?.ApplicationMethod?.ToString(),

                // Balance
                Balance_MinAmount = balanceTier?.MinimumTierValue?.Amount,
                Balance_MinCurrency = balanceTier?.MinimumTierValue?.Currency,
                Balance_MaxCurrency = balanceTier?.MinimumTierValue?.Currency,
                Balance_MaxAmount = balanceTier?.MaximumTierValue?.Amount,
                Balance_TierRate = balanceTier?.TierRate,
                // LTVTierDetails
                LTV_Start = LvtTier?.LTVStart,
                LTV_End = LvtTier?.LTVEnd,
                LTV_TierRate = LvtTier?.TierRate,

                RateRange_MaxRate = tier?.RateRange?.MaximumRate,
                RateRange_MinRate = tier?.RateRange?.MinimumRate,
                RateRange_AdditionalInformation = tier?.RateRange?.AdditionalInformation,
                Condition_Field = Conditions?.Field,
                Condition_Description = Conditions?.Description,
                Condition_Operator = Conditions?.Operator,
                Condition_Value = Conditions?.Value,
                Notes = rateOption?.Notes,
                AddInfo_Type = Additionalinfo?.Type.ToString(),
                AddInfo_Description = Additionalinfo?.Description

            };
            if (rateOption?.FixedRate != null && src?.RateType?.Contains("Fixed", StringComparison.OrdinalIgnoreCase) == true)
            {
                finance.Fixed_Description = rateOption.FixedRate.Description;
                finance.Fixed_Rate = rateOption.FixedRate.Rate;
                finance.Fixed_EndDate = rateOption.FixedRate.FixedRateEndDate;
                finance.Fixed_CalculationFrequency = rateOption.FixedRate.CalculationFrequency;
                finance.Fixed_ApplicationFrequency = rateOption.FixedRate.ApplicationFrequency;
                finance.Fixed_ProfitCalculationMethod = rateOption.FixedRate.ProfitCalculationMethod;

                finance.Fixed_APR_StartingFrom = rateOption.FixedRate.AnnualPercentageRate?.StartingFrom?.ToString();
                finance.Fixed_APR_UpTo = rateOption.FixedRate.AnnualPercentageRate?.UpTo;
                finance.Fixed_APR_AdditionalInformation = rateOption.FixedRate.AnnualPercentageRate?.AdditionalInformation;
                finance.Fixed_Tier_Name = FixedTier?.Name;
                finance.Fixed_Tier_Unit = FixedTier?.Unit?.ToString();
                finance.Fixed_Tier_ApplicationMethod = FixedTier?.ApplicationMethod?.ToString();

                // Balance
                finance.Fixed_Balance_MinAmount = FixedbalanceTier?.MinimumTierValue?.Amount;
                finance.Fixed_Balance_MaxAmount = FixedbalanceTier?.MaximumTierValue?.Amount;
                finance.Fixed_Balance_MinCurrency = balanceTier?.MinimumTierValue?.Currency;
                finance.Fixed_Balance_MaxCurrency = balanceTier?.MaximumTierValue?.Currency;
                finance.Fixed_Balance_TierRate = FixedbalanceTier?.TierRate;
                // LTVTierDetails
                finance.Fixed_LTV_Start = FixedLvtTier?.LTVStart;
                finance.Fixed_LTV_End = FixedLvtTier?.LTVEnd;
                finance.Fixed_LTV_TierRate = FixedLvtTier?.TierRate;

                finance.Fixed_RateRange_MaxRate = FixedTier?.RateRange?.MaximumRate;
                finance.Fixed_RateRange_MinRate = FixedTier?.RateRange?.MinimumRate;
                finance.Fixed_RateRange_AdditionalInformation = FixedTier?.RateRange?.AdditionalInformation;
                finance.Fixed_Condition_Field = Conditions?.Field;
                finance.Fixed_Condition_Description = Conditions?.Description;
                finance.Fixed_Condition_Operator = Conditions?.Operator;
                finance.Fixed_Condition_Value = Conditions?.Value;
                finance.Notes = rateOption?.Notes;
                finance.Fixed_AddInfo_Type = Additionalinfo?.Type.ToString();
                finance.Fixed_AddInfo_Description = Additionalinfo?.Description;
                finance.FixedRateEnd = rateOption?.FixedRate.FixedRateEnd;
                finance.Intro_Period = FixedIntroductoryPeriodOptions?.Period;
                finance.Intro_Indicative_Start = FixedIntroductoryPeriodOptions?.IndicativeRate?.StartingFrom;
                finance.Intro_Indicative_End = FixedIntroductoryPeriodOptions?.IndicativeRate?.UpTo;
            }
            // 🔹 VARIABLE
            else if (rateOption?.VariableRate != null)  
            {
                finance.Variable_Description = rateOption.VariableRate.Description;
                finance.Variable_Rate = rateOption.VariableRate.Rate;

                finance.Variable_CalculationFrequency = rateOption.VariableRate.CalculationFrequency;
                finance.Variable_ApplicationFrequency = rateOption.VariableRate.ApplicationFrequency;
                finance.Variable_ProfitCalculationMethod = rateOption.VariableRate.ProfitCalculationMethod;
                finance.Variable_Benchmark = rateOption.VariableRate.BenchMark;
                finance.Variable_BenchmarkRate = rateOption.VariableRate.BenchMarkRate;
                finance.Variable_Margin = rateOption.VariableRate.Margin;

                finance.Variable_APR_StartingFrom = rateOption.VariableRate.AnnualPercentageRate?.StartingFrom?.ToString();
                finance.Variable_APR_UpTo = rateOption.VariableRate.AnnualPercentageRate?.UpTo;
                finance.Variable_APR_AdditionalInformation = rateOption?.VariableRate?.AnnualPercentageRate?.AdditionalInformation;

                finance.Variable_Tier_Name = VariableTiers?.Name;
                finance.Variable_Tier_Unit = VariableTiers?.Unit?.ToString();
                finance.Variable_Tier_ApplicationMethod = VariableTiers?.ApplicationMethod?.ToString();

                // Balance
                finance.Variable_Balance_MinAmount = VariablebalanceTier?.MinimumTierValue?.Amount;
                finance.Variable_Balance_MaxAmount = VariablebalanceTier?.MaximumTierValue?.Amount;
                finance.Variable_Balance_MinCurrency = balanceTier?.MinimumTierValue?.Currency;
                finance.Variable_Balance_MaxCurrency = balanceTier?.MaximumTierValue?.Currency;
                finance.Variable_Balance_TierRate = VariablebalanceTier?.TierRate;
                // LTVTierDetails
                finance.Variable_LTV_Start = VariableLvtTier?.LTVStart;
                finance.Variable_LTV_End = VariableLvtTier?.LTVEnd;
                finance.Variable_LTV_TierRate = VariableLvtTier?.TierRate;

                finance.Variable_RateRange_MaxRate = VariableTiers?.RateRange?.MaximumRate;
                finance.Variable_RateRange_MinRate = VariableTiers?.RateRange?.MinimumRate;
                finance.Variable_RateRange_AdditionalInformation = VariableTiers?.RateRange?.AdditionalInformation;
                finance.Variable_Condition_Field = Conditions?.Field;
                finance.Variable_Condition_Description = Conditions?.Description;
                finance.Variable_Condition_Operator = Conditions?.Operator;
                finance.Variable_Condition_Value = Conditions?.Value;
                finance.Notes = rateOption?.Notes;
                finance.Variable_AddInfo_Type = Additionalinfo?.Type.ToString();
                finance.Variable_AddInfo_Description = Additionalinfo?.Description;
                finance.Variable_Term = rateOption?.VariableRate.VariableTerm;
            }
            return new List<FinanceRates> { finance };

        }
        public static ICollection<OF.ProductData.Model.EFModel.Products.Tenor> MapTenor(ProductData.Model.CentralBank.Products.Tenor src, long paymentRequestId)
        {
            if (src == null)
                return new List<OF.ProductData.Model.EFModel.Products.Tenor>(); 

            return new List<OF.ProductData.Model.EFModel.Products.Tenor>
            {
                    new OF.ProductData.Model.EFModel.Products.Tenor
                    {
                        MinimumTenor = src.MinimumTenor,  
                        MaximumTenor = src.MaximumTenor,
                        Condition = src.Condition
                    }
            };
        }
        public static ICollection<OF.ProductData.Model.EFModel.Products.AssetBacked> MapAssetBacked(OF.ProductData.Model.CentralBank.Products.AssetBacked src, long paymentRequestId)
        {
            if (src == null)
                return null;

            var assetBacked = new OF.ProductData.Model.EFModel.Products.AssetBacked
            {
                RequestId = paymentRequestId,
                Type = src.Type.ToString(),
                AssetType = src.AssetType.ToString(),
                Description = src.Description
            };

            if (src.Valuation != null)
            {
                var valuation = src.Valuation.FirstOrDefault();
                assetBacked.ValuationDate = valuation?.Date;
                if (valuation != null && valuation.Amount != null)
                {
                    assetBacked.ValuationAmount = valuation.Amount.Amount;
                    assetBacked.ValuationCurrency = valuation.Amount.Currency;
                }
            }

            if (src.SupplementaryInformation != null)
            {
                assetBacked.SupplementaryInformation = System.Text.Json.JsonSerializer.Serialize(src.SupplementaryInformation);
            }

            if (src.OwnershipTransfer != null)
            {
                assetBacked.TransferOfOwnershipDate = src.OwnershipTransfer.TransferOfOwnershipDate;
                assetBacked.OwnershipType = src.OwnershipTransfer.Type.ToString();
                assetBacked.OwnershipMethod = src.OwnershipTransfer.Method?.ToString();

                // Map TokenPurchaseAmount if it exists
                if (src.OwnershipTransfer.TokenPurchaseAmount != null)
                {
                    assetBacked.TokenPurchaseAmount = src.OwnershipTransfer.TokenPurchaseAmount.Amount;
                    assetBacked.TokenPurchaseCurrency = src.OwnershipTransfer.TokenPurchaseAmount.Currency;
                }

                // Map BuyoutSchedule
                if (src.OwnershipTransfer.BuyoutSchedule != null)
                {
                    assetBacked.BuyoutFrequency = src.OwnershipTransfer.BuyoutSchedule.Frequency.ToString();
                    if (src.OwnershipTransfer.BuyoutSchedule.BuyoutAmount != null)
                    {
                        assetBacked.BuyoutAmount = src.OwnershipTransfer.BuyoutSchedule.BuyoutAmount.Amount;
                        assetBacked.BuyoutCurrency = src.OwnershipTransfer.BuyoutSchedule.BuyoutAmount.Currency;
                    }
                }

                // Map SaleAgreement
                if (src.OwnershipTransfer.SaleAgreement != null)
                {
                    assetBacked.SaleRequired = src.OwnershipTransfer.SaleAgreement.Required;
                    assetBacked.SaleExecution = src.OwnershipTransfer.SaleAgreement.Execution.ToString();

                }
                if (src.OwnershipTransfer.SaleAgreement?.Price != null)
                {
                    assetBacked.SalePriceAmount = src.OwnershipTransfer.SaleAgreement?.Price?.Amount;
                    assetBacked.SalePriceCurrency = src.OwnershipTransfer.SaleAgreement?.Price?.Currency;
                }
              
                if (src.OwnershipTransfer.TransferConditions != null && src.OwnershipTransfer.TransferConditions.Any())
                {
                    assetBacked.TransferCondition = string.Join(",", src.OwnershipTransfer.TransferConditions);
                }
            }
            return new List<OF.ProductData.Model.EFModel.Products.AssetBacked> { assetBacked };
        }
        public static ICollection<ProductData.Model.EFModel.Products.RewardsBenefits> MapRewardsBenefits(ProductData.Model.CentralBank.Products.RewardsBenefits src, long paymentRequestId)
        {
            if (src == null)
            return null;
            return new List<ProductData.Model.EFModel.Products.RewardsBenefits>
                {
                    new ProductData.Model.EFModel.Products.RewardsBenefits
                    {
                        Name = src.Name,
                        Description = src.Description,
                        Type = src.Type.ToString(),
                        RewardBasis = src.RewardBasis != null && src.RewardBasis.Any()
                        ? string.Join(",", src.RewardBasis)
                        : null,
                    }
            };

        }
    }
}

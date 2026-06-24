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

        public static ICollection<FinanceRates> MapFinanaceRate(ProductFinanceRate src, long paymentRequestId)
        {
            if (src == null)
                return null;

            var finance = new FinanceRates
            {
                RequestId = paymentRequestId,
                FinanceRateType = src.RateType.ToString(),
                RateOption_RateType = src.RateType.ToString()
            };

            switch (src)
            {
                // ================= FIXED INTEREST =================
                case FixedInterest fixedInterest:
                    MapBaseFinanceRate(finance, fixedInterest);

                    finance.Fixed_Description = fixedInterest.Description;
                    finance.Fixed_Rate = fixedInterest.Rate;
                    finance.Fixed_EndDate = fixedInterest.FixedRateEndDate;
                    finance.Fixed_CalculationFrequency = fixedInterest.CalculationFrequency?.ToString();
                    finance.Fixed_ApplicationFrequency = fixedInterest.ApplicationFrequency?.ToString();
                    finance.Fixed_ProfitCalculationMethod = fixedInterest.InterestCalculationMethod?.ToString();
                    break;

                // ================= FIXED PROFIT =================
                case FixedProfit fixedProfit:
                    MapBaseFinanceRate(finance, fixedProfit);

                    finance.Fixed_Description = fixedProfit.Description;
                    finance.Fixed_Rate = fixedProfit.Rate;
                    finance.Fixed_EndDate = fixedProfit.FixedRateEndDate;
                    finance.Fixed_CalculationFrequency = fixedProfit.CalculationFrequency?.ToString();
                    finance.Fixed_ApplicationFrequency = fixedProfit.ApplicationFrequency?.ToString();
                    finance.Fixed_ProfitCalculationMethod = fixedProfit.ProfitCalculationMethod?.ToString();
                    break;

                // ================= VARIABLE INTEREST =================
                case VariableInterest variableInterest:
                    MapBaseFinanceRate(finance, variableInterest);

                    finance.Variable_Description = variableInterest.Description;
                    finance.Variable_Rate = variableInterest.Rate;
                    finance.Variable_CalculationFrequency = variableInterest.CalculationFrequency?.ToString();
                    finance.Variable_ApplicationFrequency = variableInterest.ApplicationFrequency?.ToString();
                    finance.Variable_Benchmark = variableInterest.BenchMark;
                    finance.Variable_BenchmarkRate = variableInterest.BenchMarkRate;
                    finance.Variable_Margin = variableInterest.Margin;
                    finance.Variable_ProfitCalculationMethod = variableInterest.InterestCalculationMethod?.ToString();

                    finance.Variable_ReviewFrequency = variableInterest.RateReviewFrequency;
                    finance.Variable_ReviewNextDate = variableInterest.RateReviewNextDate;
                    break;

                // ================= VARIABLE PROFIT =================
                case VariableProfit variableProfit:
                    MapBaseFinanceRate(finance, variableProfit);

                    finance.Variable_Description = variableProfit.Description;
                    finance.Variable_Rate = variableProfit.Rate;
                    finance.Variable_CalculationFrequency = variableProfit.CalculationFrequency?.ToString();
                    finance.Variable_ApplicationFrequency = variableProfit.ApplicationFrequency?.ToString();
                    finance.Variable_Benchmark = variableProfit.BenchMark;
                    finance.Variable_BenchmarkRate = variableProfit.BenchMarkRate;
                    finance.Variable_Margin = variableProfit.Margin;
                    finance.Variable_ProfitCalculationMethod = variableProfit.ProfitCalculationMethod?.ToString();

                    finance.Variable_ReviewFrequency = variableProfit.RateReviewFrequency;
                    finance.Variable_ReviewNextDate = variableProfit.RateReviewNextDate;
                    break;

                // ================= HYBRID INTEREST =================
                case HybridInterest hybridInterest:
                    MapBaseFinanceRate(finance, hybridInterest);

                    if (hybridInterest.FixedRate != null)
                    {
                        var fixedTier = hybridInterest.FixedRate.Tiers?.FirstOrDefault();
                        var fixedBalance = fixedTier?.BalanceTierDetails?.FirstOrDefault();
                        var fixedLtv = fixedTier?.LTVTierDetails?.FirstOrDefault();
                        var fixedCondition = hybridInterest.FixedRate.Conditions?.FirstOrDefault();
                        var fixedAddInfo = hybridInterest.FixedRate.AdditionalInformation?.FirstOrDefault();

                        finance.Fixed_Description = hybridInterest.FixedRate.Description;
                        finance.Fixed_Rate = hybridInterest.FixedRate.Rate;
                        finance.Fixed_EndDate = hybridInterest.FixedRate.FixedRateEndDate;
                        finance.Fixed_CalculationFrequency = hybridInterest.FixedRate.CalculationFrequency?.ToString();
                        finance.Fixed_ApplicationFrequency = hybridInterest.FixedRate.ApplicationFrequency?.ToString();
                        finance.Fixed_ProfitCalculationMethod = hybridInterest.FixedRate.InterestCalculationMethod?.ToString();
                        finance.FixedRateEnd = hybridInterest.FixedRate.FixedRateEnd;

                        finance.Fixed_APR_StartingFrom = hybridInterest.FixedRate.AnnualPercentageRate?.StartingFrom;
                        finance.Fixed_APR_UpTo = hybridInterest.FixedRate.AnnualPercentageRate?.UpTo;
                        finance.Fixed_APR_AdditionalInformation = hybridInterest.FixedRate.AnnualPercentageRate?.AdditionalInformation;

                        finance.Fixed_Tier_Name = fixedTier?.Name;
                        finance.Fixed_Tier_Unit = fixedTier?.Unit?.ToString();
                        finance.Fixed_Tier_ApplicationMethod = fixedTier?.ApplicationMethod?.ToString();

                        finance.Fixed_Balance_MinAmount = fixedBalance?.MinimumTierValue?.Amount;
                        finance.Fixed_Balance_MaxAmount = fixedBalance?.MaximumTierValue?.Amount;
                        finance.Fixed_Balance_MinCurrency = fixedBalance?.MinimumTierValue?.Currency;
                        finance.Fixed_Balance_MaxCurrency = fixedBalance?.MaximumTierValue?.Currency;
                        finance.Fixed_Balance_TierRate = fixedBalance?.TierRate;

                        finance.Fixed_LTV_Start = fixedLtv?.LTVStart;
                        finance.Fixed_LTV_End = fixedLtv?.LTVEnd;
                        finance.Fixed_LTV_TierRate = fixedLtv?.TierRate;

                        finance.Fixed_Condition_Field = fixedCondition?.Field;
                        finance.Fixed_Condition_Operator = fixedCondition?.Operator;
                        finance.Fixed_Condition_Value = fixedCondition?.Value;
                        finance.Fixed_Condition_Description = fixedCondition?.Description;

                        finance.Fixed_AddInfo_Type = fixedAddInfo?.Type?.ToString();
                        finance.Fixed_AddInfo_Description = fixedAddInfo?.Description;

                        finance.Fixed_RateRange_MinRate = fixedTier?.RateRange?.MinimumRate;
                        finance.Fixed_RateRange_MaxRate = fixedTier?.RateRange?.MaximumRate;
                        finance.Fixed_RateRange_AdditionalInformation = fixedTier?.RateRange?.AdditionalInformation;
                    }

                    if (hybridInterest.VariableRate != null)
                    {
                        var variableTier = hybridInterest.VariableRate.Tiers?.FirstOrDefault();
                        var variableBalance = variableTier?.BalanceTierDetails?.FirstOrDefault();
                        var variableLtv = variableTier?.LTVTierDetails?.FirstOrDefault();
                        var variableCondition = hybridInterest.VariableRate.Conditions?.FirstOrDefault();
                        var variableAddInfo = hybridInterest.VariableRate.AdditionalInformation?.FirstOrDefault();

                        finance.Variable_Description = hybridInterest.VariableRate.Description;
                        finance.Variable_Rate = hybridInterest.VariableRate.Rate;
                        finance.Variable_CalculationFrequency = hybridInterest.VariableRate.CalculationFrequency?.ToString();
                        finance.Variable_ApplicationFrequency = hybridInterest.VariableRate.ApplicationFrequency?.ToString();
                        finance.Variable_Benchmark = hybridInterest.VariableRate.BenchMark;
                        finance.Variable_BenchmarkRate = hybridInterest.VariableRate.BenchMarkRate;
                        finance.Variable_Margin = hybridInterest.VariableRate.Margin;
                        finance.Variable_ProfitCalculationMethod = hybridInterest.VariableRate.InterestCalculationMethod?.ToString();
                        finance.Variable_Term = hybridInterest.VariableRate.VariableTerm;

                        finance.Variable_ReviewFrequency = hybridInterest.VariableRate.RateReviewFrequency;
                        finance.Variable_ReviewNextDate = hybridInterest.VariableRate.RateReviewNextDate;

                        finance.Variable_APR_StartingFrom = hybridInterest.VariableRate.AnnualPercentageRate?.StartingFrom;
                        finance.Variable_APR_UpTo = hybridInterest.VariableRate.AnnualPercentageRate?.UpTo;
                        finance.Variable_APR_AdditionalInformation = hybridInterest.VariableRate.AnnualPercentageRate?.AdditionalInformation;

                        finance.Variable_Tier_Name = variableTier?.Name;
                        finance.Variable_Tier_Unit = variableTier?.Unit?.ToString();
                        finance.Variable_Tier_ApplicationMethod = variableTier?.ApplicationMethod?.ToString();

                        finance.Variable_Balance_MinAmount = variableBalance?.MinimumTierValue?.Amount;
                        finance.Variable_Balance_MaxAmount = variableBalance?.MaximumTierValue?.Amount;
                        finance.Variable_Balance_MinCurrency = variableBalance?.MinimumTierValue?.Currency;
                        finance.Variable_Balance_MaxCurrency = variableBalance?.MaximumTierValue?.Currency;
                        finance.Variable_Balance_TierRate = variableBalance?.TierRate;

                        finance.Variable_LTV_Start = variableLtv?.LTVStart;
                        finance.Variable_LTV_End = variableLtv?.LTVEnd;
                        finance.Variable_LTV_TierRate = variableLtv?.TierRate;

                        finance.Variable_RateRange_MinRate = variableTier?.RateRange?.MinimumRate;
                        finance.Variable_RateRange_MaxRate = variableTier?.RateRange?.MaximumRate;
                        finance.Variable_RateRange_AdditionalInformation = variableTier?.RateRange?.AdditionalInformation;

                        finance.Variable_Condition_Field = variableCondition?.Field;
                        finance.Variable_Condition_Operator = variableCondition?.Operator;
                        finance.Variable_Condition_Value = variableCondition?.Value;
                        finance.Variable_Condition_Description = variableCondition?.Description;

                        finance.Variable_AddInfo_Type = variableAddInfo?.Type?.ToString();
                        finance.Variable_AddInfo_Description = variableAddInfo?.Description;
                    }
                    break;

                // ================= HYBRID PROFIT =================
                case HybridProfit hybridProfit:
                    MapBaseFinanceRate(finance, hybridProfit);

                    if (hybridProfit.FixedRate != null)
                    {
                        finance.Fixed_Description = hybridProfit.FixedRate.Description;
                        finance.Fixed_Rate = hybridProfit.FixedRate.Rate;
                        finance.Fixed_EndDate = hybridProfit.FixedRate.FixedRateEndDate;
                        finance.Fixed_CalculationFrequency = hybridProfit.FixedRate.CalculationFrequency?.ToString();
                        finance.Fixed_ApplicationFrequency = hybridProfit.FixedRate.ApplicationFrequency?.ToString();
                        finance.Fixed_ProfitCalculationMethod = hybridProfit.FixedRate.ProfitCalculationMethod?.ToString();
                        finance.FixedRateEnd = hybridProfit.FixedRate.FixedRateEnd;
                    }

                    if (hybridProfit.VariableRate != null)
                    {
                        finance.Variable_Description = hybridProfit.VariableRate.Description;
                        finance.Variable_Rate = hybridProfit.VariableRate.Rate;
                        finance.Variable_CalculationFrequency = hybridProfit.VariableRate.CalculationFrequency?.ToString();
                        finance.Variable_ApplicationFrequency = hybridProfit.VariableRate.ApplicationFrequency?.ToString();
                        finance.Variable_Benchmark = hybridProfit.VariableRate.BenchMark;
                        finance.Variable_BenchmarkRate = hybridProfit.VariableRate.BenchMarkRate;
                        finance.Variable_Margin = hybridProfit.VariableRate.Margin;
                        finance.Variable_ProfitCalculationMethod = hybridProfit.VariableRate.ProfitCalculationMethod?.ToString();
                        finance.Variable_Term = hybridProfit.VariableRate.VariableTerm;
                    }
                    break;

                // ================= INTEREST RATE OPTIONS =================
                case InterestRateOptions interestOptions:
                    var interestOption = interestOptions.RateOptions?.FirstOrDefault();

                    switch (interestOption)
                    {
                        case FixedInterestRateOption fixedOption:
                            var tier = fixedOption.Tiers?.FirstOrDefault();
                            var balance = tier?.BalanceTierDetails?.FirstOrDefault();
                            var ltv = tier?.LTVTierDetails?.FirstOrDefault();
                            var condition = fixedOption.Conditions?.FirstOrDefault();
                            var addInfo = fixedOption.AdditionalInformation?.FirstOrDefault();
                            var intro = fixedOption.IntroductoryPeriodOptions?.FirstOrDefault();

                            finance.Fixed_Description = fixedOption.Description;
                            finance.Fixed_Rate = fixedOption.Rate;
                            finance.Fixed_EndDate = fixedOption.FixedRateEndDate;
                            finance.Fixed_CalculationFrequency = fixedOption.CalculationFrequency?.ToString();
                            finance.Fixed_ApplicationFrequency = fixedOption.ApplicationFrequency?.ToString();
                            finance.Fixed_ProfitCalculationMethod = fixedOption.InterestCalculationMethod?.ToString();

                            // APR
                            finance.Fixed_APR_StartingFrom = fixedOption.AnnualPercentageRate?.StartingFrom;
                            finance.Fixed_APR_UpTo = fixedOption.AnnualPercentageRate?.UpTo;
                            finance.Fixed_APR_AdditionalInformation = fixedOption.AnnualPercentageRate?.AdditionalInformation;

                            // Tier
                            finance.Fixed_Tier_Name = tier?.Name;
                            finance.Fixed_Tier_Unit = tier?.Unit?.ToString();
                            finance.Fixed_Tier_ApplicationMethod = tier?.ApplicationMethod?.ToString();

                            // Balance Tier
                            finance.Fixed_Balance_MinAmount = balance?.MinimumTierValue?.Amount;
                            finance.Fixed_Balance_MinCurrency = balance?.MinimumTierValue?.Currency;
                            finance.Fixed_Balance_MaxAmount = balance?.MaximumTierValue?.Amount;
                            finance.Fixed_Balance_MaxCurrency = balance?.MaximumTierValue?.Currency;
                            finance.Fixed_Balance_TierRate = balance?.TierRate;

                            // LTV Tier
                            finance.Fixed_LTV_Start = ltv?.LTVStart;
                            finance.Fixed_LTV_End = ltv?.LTVEnd;
                            finance.Fixed_LTV_TierRate = ltv?.TierRate;

                            // Rate Range
                            finance.Fixed_RateRange_MinRate = tier?.RateRange?.MinimumRate;
                            finance.Fixed_RateRange_MaxRate = tier?.RateRange?.MaximumRate;
                            finance.Fixed_RateRange_AdditionalInformation =
                                tier?.RateRange?.AdditionalInformation;

                            // Condition
                            finance.Fixed_Condition_Field = condition?.Field;
                            finance.Fixed_Condition_Operator = condition?.Operator;
                            finance.Fixed_Condition_Value = condition?.Value;
                            finance.Fixed_Condition_Description = condition?.Description;

                            // Additional Info
                            finance.Fixed_AddInfo_Type = addInfo?.Type?.ToString();
                            finance.Fixed_AddInfo_Description = addInfo?.Description;

                            // Notes
                            finance.Notes = fixedOption.Notes;

                            // Introductory Period
                            finance.Intro_Period = intro?.Period;
                            finance.Intro_Indicative_Start = intro?.IndicativeRate?.StartingFrom;
                            finance.Intro_Indicative_End = intro?.IndicativeRate?.UpTo;
                            break;

                        case VariableInterestRateOption variableOption:
                            finance.Variable_Description = variableOption.Description;
                            finance.Variable_Rate = variableOption.Rate;
                            finance.Variable_Benchmark = variableOption.BenchMark;
                            finance.Variable_BenchmarkRate = variableOption.BenchMarkRate;
                            finance.Variable_Margin = variableOption.Margin;

                            finance.Variable_ReviewFrequency = variableOption.RateReviewFrequency;
                            finance.Variable_ReviewNextDate = variableOption.RateReviewNextDate;
                            break;

                        case HybridInterestRateOption hybridOption:
                            finance.Fixed_Description = hybridOption.FixedRate?.Description;
                            finance.Variable_Description = hybridOption.VariableRate?.Description;
                            break;
                    }
                    break;

                // ================= PROFIT RATE OPTIONS =================
                case ProfitRateOptions profitOptions:
                    var profitOption = profitOptions.RateOptions?.FirstOrDefault();

                    switch (profitOption)
                    {
                        case FixedProfitRateOption fixedOption:
                            finance.Fixed_Description = fixedOption.Description;
                            finance.Fixed_Rate = fixedOption.Rate;
                            finance.Fixed_EndDate = fixedOption.FixedRateEndDate;
                            finance.Fixed_CalculationFrequency = fixedOption.CalculationFrequency?.ToString();
                            finance.Fixed_ApplicationFrequency = fixedOption.ApplicationFrequency?.ToString();
                            finance.Fixed_ProfitCalculationMethod = fixedOption.ProfitCalculationMethod?.ToString();
                            break;

                        case VariableProfitRateOption variableOption:
                            finance.Variable_Description = variableOption.Description;
                            finance.Variable_Rate = variableOption.Rate;
                            finance.Variable_Benchmark = variableOption.BenchMark;
                            finance.Variable_BenchmarkRate = variableOption.BenchMarkRate;
                            finance.Variable_Margin = variableOption.Margin;
                            break;

                        case HybridProfitRateOption hybridOption:
                            finance.Fixed_Description = hybridOption.FixedRate?.Description;
                            finance.Variable_Description = hybridOption.VariableRate?.Description;
                            break;
                    }
                    break;
            }

            return new List<FinanceRates> { finance };
        }

        private static void MapBaseFinanceRate(FinanceRates finance, FinanceRateBase baseRate)
        {
            var tier = baseRate.Tiers?.FirstOrDefault();
            var balanceTier = tier?.BalanceTierDetails?.FirstOrDefault();
            var ltvTier = tier?.LTVTierDetails?.FirstOrDefault();
            var condition = baseRate.Conditions?.FirstOrDefault();
            var additionalInfo = baseRate.AdditionalInformation?.FirstOrDefault();

            finance.APR_StartingFrom = baseRate.AnnualPercentageRate?.StartingFrom;
            finance.APR_UpTo = baseRate.AnnualPercentageRate?.UpTo;
            finance.APR_AdditionalInformation = baseRate.AnnualPercentageRate?.AdditionalInformation;

            finance.Tier_Name = tier?.Name;
            finance.Tier_Unit = tier?.Unit?.ToString();
            finance.Tier_ApplicationMethod = tier?.ApplicationMethod?.ToString();

            finance.Balance_MinAmount = balanceTier?.MinimumTierValue?.Amount;
            finance.Balance_MinCurrency = balanceTier?.MinimumTierValue?.Currency;
            finance.Balance_MaxAmount = balanceTier?.MaximumTierValue?.Amount;
            finance.Balance_MaxCurrency = balanceTier?.MaximumTierValue?.Currency;
            finance.Balance_TierRate = balanceTier?.TierRate;

            finance.LTV_Start = ltvTier?.LTVStart;
            finance.LTV_End = ltvTier?.LTVEnd;
            finance.LTV_TierRate = ltvTier?.TierRate;

            finance.RateRange_MaxRate = tier?.RateRange?.MaximumRate;
            finance.RateRange_MinRate = tier?.RateRange?.MinimumRate;
            finance.RateRange_AdditionalInformation = tier?.RateRange?.AdditionalInformation;

            finance.Condition_Field = condition?.Field;
            finance.Condition_Description = condition?.Description;
            finance.Condition_Operator = condition?.Operator;
            finance.Condition_Value = condition?.Value;

            finance.Notes = baseRate.Notes;

            finance.AddInfo_Type = additionalInfo?.Type?.ToString();
            finance.AddInfo_Description = additionalInfo?.Description;
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
                        BalanceAmount = src.Balance?.Amount,
                        BalanceCurrency = src.Balance?.Currency,
                        FrequencyPaid = src.FrequencyPaid?.ToString(),
                        PointsType = src.PointsType,
                        ExpiryDate = src.ExpiryDate
                    }
            };

        }
    }
}

using Newtonsoft.Json;
using OF.ProductData.Model.CentralBank.Products;
using OF.ProductData.Model.EFModel.Products;

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
                        ResidenceStatusType = prod.Eligibility?.ResidenceStatus?.FirstOrDefault()?.Type,
                        ResidenceStatusDescription = prod.Eligibility?.ResidenceStatus?.FirstOrDefault()?.Description,
                        EmploymentStatusType = prod.Eligibility?.EmploymentStatus?.FirstOrDefault()?.Type,
                        EmploymentStatusDescription = prod.Eligibility?.EmploymentStatus?.FirstOrDefault()?.Description,
                        CustomerTypeType = prod.Eligibility?.CustomerType?.FirstOrDefault()?.Type,
                        CustomerTypeDescription = prod.Eligibility?.CustomerType?.FirstOrDefault()?.Description,
                        AccountOwnershipType = prod.Eligibility?.AccountOwnership?.FirstOrDefault()?.Type,
                        AccountOwnershipDescription = prod.Eligibility?.AccountOwnership?.FirstOrDefault()?.Description,
                        AgeEligibilityType = prod.Eligibility?.Age?.FirstOrDefault()?.Type,
                        AgeEligibilityValue = prod.Eligibility?.Age?.FirstOrDefault()?.Value ?? 0,
                        AgeEligibilityDescription = prod.Eligibility?.Age?.FirstOrDefault()?.Description,
                        AdditionalEligibilityType = prod.Eligibility?.AdditionalEligibility?.FirstOrDefault()?.Type,
                        AdditionalEligibilityDescription = prod.Eligibility?.AdditionalEligibility?.FirstOrDefault()?.Description,

                        // Nested product types
                        CurrentAccount = MapCurrentAccount(prod.Product?.CurrentAccount!, paymentRequestId),
                        SavingsAccount = MapSavingsAccount(prod.Product?.SavingsAccount!, paymentRequestId),
                        CreditCard = MapCreditCard(prod.Product?.CreditCard!, paymentRequestId),
                        PersonalLoan = MapPersonalLoan(prod.Product?.PersonalLoan!, paymentRequestId),
                        Mortgage = MapMortgage(prod.Product?.Mortgage!, paymentRequestId),
                        DepositRates = MapDepositRates(prod.Product?.depositRates!, paymentRequestId),
                        FinanceRateMapped = MapFinanceInterestRate(prod.Product?.FinanceInterestRate!, paymentRequestId),
                        TenorMapped = MapTenor(prod.Product!.Tenor!, paymentRequestId),
                        AssetBackedMapped = MapAssetBacked(prod.Product.AssetBacked!, paymentRequestId),
                        RewardsMapped = MapRewardsBenefits(prod.Product.RewardsBenefits!, paymentRequestId),
                        //ProfitSharingRate = MapProfitSharingRate(prod.Product?.ProfitSharingRate, paymentRequestId),
                        //FinanceProfitRate = MapFinanceProfitRate(prod.Product?.FinanceProfitRate, paymentRequestId)
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
            var fee = src.Fees?.FirstOrDefault();
            return new List<CurrentAccounts>
    {
     new CurrentAccounts
        {
         RequestId = paymentRequestId,
            Type = src.Type,
            Description = src.Description,
             IsOverdraftAvailable = src.IsOverdraftAvailable,
              // Documentation
                DocumentationType = src.Documentation?.FirstOrDefault()?.Type,
                DocumentationDescription = src.Documentation?.FirstOrDefault()?.Description,

                // Features
                FeaturesType = src.Features?.FirstOrDefault()?.Type,
                FeaturesDescription = src.Features?.FirstOrDefault()?.Description,

                // Fees
                FeesType = src.Fees?.FirstOrDefault()?.Type,
                FeesPeriod = src.Fees?.FirstOrDefault()?.Period,
                FeesName = src.Fees?.FirstOrDefault()?.Name,
                FeesDescription = src.Fees?.FirstOrDefault()?.Description,
                FeesUnit = src.Fees?.FirstOrDefault()?.Unit,
                FeesAmount = src.Fees?.FirstOrDefault()?.Amount?.AmountValue != null
    ? Convert.ToDecimal(src.Fees.FirstOrDefault().Amount.AmountValue)
    : (decimal?)null,
                FeesCurrency = src.Fees?.FirstOrDefault()?.Amount?.Currency,

                FeesPercentage = (decimal?)fee?.Percentage,
                FeesUnitValue = (decimal?)fee?.UnitValue,
                FeesMaximumUnitValue = (decimal?)fee?.MaximumUnitValue,

                // Limits
                LimitsType = src.Limits?.FirstOrDefault()?.Type,
                LimitsDescription = src.Limits?.FirstOrDefault()?.Description,
                LimitsValue = src.Limits?.FirstOrDefault() != null ? (decimal?)src.Limits.FirstOrDefault().Value : null,

                // Benefits
                BenefitsType = src.Benefits?.FirstOrDefault()?.Type,
                BenefitsName = src.Benefits?.FirstOrDefault()?.Name,
                BenefitsDescription = src.Benefits?.FirstOrDefault()?.Description,
                BenefitsValue = src.Benefits?.FirstOrDefault() != null ? (decimal?)src.Benefits.FirstOrDefault()!.Value : null
     }
    };
        }


        public static ICollection<SavingsAccount> MapSavingsAccount(SavingsAccountData src, long paymentRequestId)
        {
            if (src == null) return null;
            var fee = src.Fees?.FirstOrDefault();
            return new List<SavingsAccount>
    {
       new SavingsAccount{
           RequestId = paymentRequestId,
                Type = src.Type,
                Description = src.Description,
                MinimumBalance=Convert.ToDecimal(src.MinimumBalance!.AmountValue),
                Currency=src.MinimumBalance.Currency,
                AnnualReturn=Convert.ToDecimal(src.AnnualReturn),
                DocumentationType = src.Documentation?.FirstOrDefault()?.Type,
                DocumentationDescription = src.Documentation?.FirstOrDefault()?.Description,

                // Features
                FeaturesType = src.Features?.FirstOrDefault()?.Type,
                FeaturesDescription = src.Features?.FirstOrDefault()?.Description,

                // Fees
                FeesType = src.Fees?.FirstOrDefault()?.Type,
                FeesPeriod = src.Fees?.FirstOrDefault()?.Period,
                FeesName = src.Fees?.FirstOrDefault()?.Name,
                FeesDescription = src.Fees?.FirstOrDefault()?.Description,
                FeesUnit = src.Fees?.FirstOrDefault()?.Unit,
                FeesAmount = src.Fees?.FirstOrDefault()?.Amount?.AmountValue != null
                ? Convert.ToDecimal(src.Fees.FirstOrDefault().Amount.AmountValue)
                : (decimal?)null,
                FeesCurrency = src.Fees?.FirstOrDefault()?.Amount?.Currency,

                FeesPercentage = (decimal?)fee?.Percentage,
                FeesUnitValue = (decimal?)fee?.UnitValue,
                FeesMaximumUnitValue = (decimal?)fee?.MaximumUnitValue,


                // Limits
                LimitsType = src.Limits?.FirstOrDefault()?.Type,
                LimitsDescription = src.Limits?.FirstOrDefault()?.Description,
                LimitsValue = src.Limits?.FirstOrDefault() != null ? (decimal?)src.Limits.FirstOrDefault().Value : null,

                // Benefits
                BenefitsType = src.Benefits?.FirstOrDefault()?.Type,
                BenefitsName = src.Benefits?.FirstOrDefault()?.Name,
                BenefitsDescription = src.Benefits?.FirstOrDefault()?.Description,
                BenefitsValue = src.Benefits?.FirstOrDefault() != null ? (decimal?)src.Benefits.FirstOrDefault().Value : null
       }
    };
        }


        public static ICollection<CreditCard> MapCreditCard(CreditCardData src, long paymentRequestId)
        {
            if (src == null) return null;

            var fee = src.Fees?.FirstOrDefault();

            return new List<CreditCard>
            {
                new CreditCard{
                    RequestId = paymentRequestId,
                Type = src.Type,
                Description = src.Description,
                Rate = src.Rate,
                DocumentationType = src.Documentation?.FirstOrDefault()?.Type,
                DocumentationDescription = src.Documentation?.FirstOrDefault()?.Description,
                FeaturesType = src.Features?.FirstOrDefault()?.Type,
                FeaturesDescription = src.Features?.FirstOrDefault()?.Description,
                FeesType = fee?.Type,
                FeesPeriod = fee?.Period,
                FeesName = fee?.Name,
                FeesDescription = fee?.Description,
                FeesUnit = fee?.Unit,
                FeesAmount = fee?.Amount?.AmountValue != null ? decimal.Parse(fee.Amount.AmountValue) : (decimal?)null,
                FeesCurrency = fee?.Amount?.Currency,
                FeesPercentage = (decimal?)fee?.Percentage,
                FeesUnitValue = (decimal?)fee?.UnitValue,
                FeesMaximumUnitValue = (decimal?)fee?.MaximumUnitValue,
                LimitsType = src.Limits?.FirstOrDefault()?.Type,
                LimitsDescription = src.Limits?.FirstOrDefault()?.Description,
                LimitsValue = src.Limits?.FirstOrDefault() != null ? (decimal?)src.Limits.FirstOrDefault().Value : null,
                BenefitsType = src.Benefits?.FirstOrDefault()?.Type,
                BenefitsName = src.Benefits?.FirstOrDefault()?.Name,
                BenefitsDescription = src.Benefits?.FirstOrDefault()?.Description,
                BenefitsValue = src.Benefits?.FirstOrDefault() != null ? (decimal?)src.Benefits.FirstOrDefault().Value : null
                }
            };
        }

        public static ICollection<PersonalLoan> MapPersonalLoan(PersonalLoanData src, long paymentRequestId)
        {
            if (src == null) return null;
            var firstBenefit = src.Benefits?.FirstOrDefault();
            return new List<PersonalLoan>
    {
          new PersonalLoan{

              RequestId = paymentRequestId,
            Type = src.Type,
            Description = src.Description,
            MinimumLoanAmount = src.MinimumLoanAmount != null ? decimal.Parse(src.MinimumLoanAmount.AmountValue) : (decimal?)null,
            MinimumLoanCurrency=src.MinimumLoanAmount.Currency,
            MaximumLoanAmount = src.MaximumLoanAmount != null ? decimal.Parse(src.MaximumLoanAmount.AmountValue) : (decimal?)null,
            MaximumLoanCurrency=src.MaximumLoanAmount.Currency,
            MaxTenure = src.Tenure.MaximumLoanTenure,
            MinTenure = src.Tenure.MaximumLoanTenure,
            CalculationMethod = src.CalculationMethod,
            RateType = src?.Type,
            RateDescription = src?.Description,
            ReviewFrequency = src?.Rate.ReviewFrequency,
            IndicativeRateFrom = src?.Rate.IndicativeRate?.From,
            IndicativeRateTo = src?.Rate.IndicativeRate?.To,
            ProfitRateFrom = src?.Rate.ProfitRate?.From,
            ProfitRateTo = src?.Rate.ProfitRate?.To,
            AnnualPercentageRateFrom = src.AnnualPercentageRateRange.From,
            AnnualPercentageRateTo   = src.AnnualPercentageRateRange.To,
            FixedRatePeriod=src?.FixedRatePeriod,
            DebtBurdenRatio=src?.DebtBurdenRatio,
            AdditionalInfoType = src.AdditionalInformation?.FirstOrDefault()?.Type,
            AdditionalInfoDescription = src.AdditionalInformation?.FirstOrDefault()?.Description,
            DocumentationType = src.Documentation?.FirstOrDefault()?.Type,
            DocumentationDescription = src.Documentation?.FirstOrDefault()?.Description,
            FeaturesType = src.Features?.FirstOrDefault()?.Type,
            FeaturesDescription = src.Features?.FirstOrDefault()?.Description,
            FeesType = src.Fees?.FirstOrDefault()?.Type,
            FeesPeriod = src.Fees?.FirstOrDefault()?.Period,
            FeesDescription = src.Fees?.FirstOrDefault()?.Description,
            FeesUnit= src.Fees?.FirstOrDefault()?.Unit,
            FeesName = src.Fees?.FirstOrDefault()?.Name,
            FeesCurrency = src.Fees?.FirstOrDefault()?.Amount.Currency,
            FeesPercentage = Convert.ToDecimal(src.Fees?.FirstOrDefault()?.Percentage),
            FeesUnitValue =Convert.ToDecimal(src.Fees?.FirstOrDefault()?.UnitValue),
            FeesMaximumUnitValue = Convert.ToDecimal(src.Fees?.FirstOrDefault()?.MaximumUnitValue),
            FeesAmount = src.Fees?.FirstOrDefault()?.Amount != null ? decimal.Parse(src.Fees.First().Amount.AmountValue) : (decimal?)null,
            LimitsDescription=src.Limits.FirstOrDefault().Description,
            LimitsType=src.Limits.FirstOrDefault().Type,
            LimitsValue = src.Limits?.FirstOrDefault() != null ? (decimal?)src.Limits.FirstOrDefault().Value : null,

                BenefitsName = firstBenefit?.Name,
                BenefitsType = firstBenefit?.Type,
                BenefitsDescription = firstBenefit?.Description,
                BenefitsValue = firstBenefit != null ? (decimal)firstBenefit.Value : (decimal?)null
                }


    };
        }


        public static ICollection<Mortgage> MapMortgage(MortgageData src, long paymentRequestId)
        {
            if (src == null) return null;
            var firstBenefit = src.Benefits?.FirstOrDefault();
            return new List<Mortgage>
    {
        new Mortgage{
            RequestId = paymentRequestId,
            Type = src.Type,
            Description = src.Description,
            CalculationMethod = src.CalculationMethod,
            Structure=src.Structure,
            MinimumLoanAmount = src.MinimumLoanAmount != null ? decimal.Parse(src.MinimumLoanAmount.AmountValue) : (decimal?)null,
            MinimumLoanCurrency=src.MinimumLoanAmount.Currency,
            MaximumLoanAmount = src.MaximumLoanAmount != null ? decimal.Parse(src.MaximumLoanAmount.AmountValue) : (decimal?)null,
            MaximumLoanCurrency=src.MaximumLoanAmount.Currency,
            MaxTenure = src.Tenure.MaximumLoanTenure,
            MinTenure = src.Tenure.MaximumLoanTenure,

            RateType = src?.Type,
            RateDescription = src?.Description,
            ReviewFrequency = src?.Rate.ReviewFrequency,
            IndicativeAPRFrom = src?.IndicativeAPR?.From,
            IndicativeAPRTo = src?.IndicativeAPR?.To,
            ProfitRateFrom = src?.Rate.ProfitRate?.From,
            ProfitRateTo = src?.Rate.ProfitRate?.To,
            IndicativeRateFrom = src?.Rate.IndicativeRate?.From,
            IndicativeRateTo = src?.Rate.IndicativeRate?.To,
            FixedRatePeriod=src?.FixedRatePeriod,
            DocumentationType = src.Documentation?.FirstOrDefault()?.Type,
            DocumentationDescription = src.Documentation?.FirstOrDefault()?.Description,
            FeaturesType = src.Features?.FirstOrDefault()?.Type,
            FeaturesDescription = src.Features?.FirstOrDefault()?.Description,
            FeesType = src.Fees?.FirstOrDefault()?.Type,
            FeesPeriod = src.Fees?.FirstOrDefault()?.Period,
            FeesDescription = src.Fees?.FirstOrDefault()?.Description,
            FeesUnit= src.Fees?.FirstOrDefault()?.Unit,
            FeesName = src.Fees?.FirstOrDefault()?.Name,
            FeesCurrency = src.Fees?.FirstOrDefault()?.Amount.Currency,
            FeesPercentage = Convert.ToDecimal(src.Fees?.FirstOrDefault()?.Percentage),
            FeesUnitValue =Convert.ToDecimal(src.Fees?.FirstOrDefault()?.UnitValue),
            FeesMaximumUnitValue = Convert.ToDecimal(src.Fees?.FirstOrDefault()?.MaximumUnitValue),
            FeesAmount = src.Fees?.FirstOrDefault()?.Amount != null ? decimal.Parse(src.Fees.First().Amount.AmountValue) : (decimal?)null,
            LimitsDescription=src.Limits.FirstOrDefault().Description,
            LimitsType=src.Limits.FirstOrDefault().Type,
            LimitsValue = src.Limits?.FirstOrDefault() != null ? (decimal?)src.Limits.FirstOrDefault().Value : null,
            BenefitsName = firstBenefit?.Name,
            BenefitsType = firstBenefit?.Type,
            BenefitsDescription = firstBenefit?.Description,
            BenefitsValue = firstBenefit != null ? (decimal)firstBenefit.Value : (decimal?)null
        }


    };
        }

        public static ICollection<ProfitSharingRate> MapProfitSharingRate(ProfitSharingRateData src, long paymentRequestId)
        {
            if (src == null) return null;

            return new List<ProfitSharingRate>
    {
       new ProfitSharingRate{
           RequestId = paymentRequestId,
            Name = src.Name,
            Description = src.Description,
            MinimumDepositAmount = src.MinimumDepositAmount?.AmountValue != null
                                    ? decimal.Parse(src.MinimumDepositAmount.AmountValue)
                                    : (decimal?)null,
            Currency = src.MinimumDepositAmount?.Currency,

            AnnualReturn = src.AnnualReturn,

            // InvestmentPeriod
            InvestmentPeriodName = src.InvestmentPeriod?.Name,
            InvestmentPeriodDescription = src.InvestmentPeriod?.Description,

            // AnnualReturnOptions - take first option if available
            AnnualReturnName = src.AnnualReturnOptions?.FirstOrDefault()?.Name,
            AnnualReturnDescription = src.AnnualReturnOptions?.FirstOrDefault()?.Description,

            // Additional Information - take first option if available
            AdditionalInfoType = src.AdditionalInformation?.FirstOrDefault()?.Type,
            AdditionalInfoDescription = src.AdditionalInformation?.FirstOrDefault()?.Description
       }
    };
        }

        public static ICollection<DepositRatesModel> MapDepositRates(
    DepositRates src,
    long paymentRequestId)
        {
            if (src == null || src.RateDetails == null || !src.RateDetails.Any())
                return null;

            var result = new List<DepositRatesModel>();

            foreach (var rate in src.RateDetails)
            {
                result.Add(new DepositRatesModel
                {
                    Id = Guid.NewGuid(),
                    RequestId = paymentRequestId,
                    RateType = src.RateType.ToString(),

                    RateCategory = rate.RateCategory.ToString(),
                    AnnualRate = rate.AnnualRate,
                    AnnualRateRangeMin = rate.AnnualRateRange?.MinRate,
                    AnnualRateRangeMax = rate.AnnualRateRange?.MaxRate,

                    TierMinBalance = rate.Tier?.MinBalance,
                    TierMaxBalance = rate.Tier?.MaxBalance,
                    TierCurrency = rate.Tier?.Currency,

                    Term = rate.Term,
                    EffectiveDate = rate.EffectiveDate,
                    ExpiryDate = rate.ExpiryDate,

                    CalculationMethod = rate.CalculationMethod.ToString(),
                    CalculationFrequency = rate.CalculationFrequency.ToString(),
                    ApplicationFrequency = rate.ApplicationFrequency.ToString(),

                    Notes = rate.Notes
                });
            }

            return result;
        }
        public OF.ProductData.Model.EFModel.Products.FinanceRates MapFinanceInterestRate(OF.ProductData.Model.CentralBank.Products.FinanceInterestRate src,
        long paymentRequestId)
        {
            if (src == null)
                return null;

            var tier = src.Tiers?.FirstOrDefault();
            var balance = tier?.BalanceTierDetails?.FirstOrDefault();
            var ltv = tier?.LTVTierDetails?.FirstOrDefault();
            var condition = src.Conditions?.FirstOrDefault();
            var additional = src.AdditionalInformation?.FirstOrDefault();

            return new OF.ProductData.Model.EFModel.Products.FinanceRates
            {
                Id = Guid.NewGuid(),
                RequestId = paymentRequestId,

                // Basic
                Description = src.Description,
                Rate = src.Rate,
                BenchMark = src.BenchMark,
                BenchMarkRate = src.BenchMarkRate,
                Margin = src.Margin,

                // Review
                RateReviewFrequency = src.RateReviewFrequency,
                RateReviewNextDate = src.RateReviewNextDate,

                // Frequency
                CalculationFrequency = src.CalculationFrequency,
                ApplicationFrequency = src.ApplicationFrequency,
                InterestCalculationMethod = src.InterestCalculationMethod,

                // APR
                AnnualPercentageRateStartingFrom = Convert.ToString(src.AnnualPercentageRate?.StartingFrom),
                AnnualPercentageRateUpTo = src.AnnualPercentageRate?.UpTo,
                AnnualPercentageRateAdditionalInfo = src.AnnualPercentageRate?.AdditionalInformation,

                // ✅ Tier 1
                Tier1Name = tier?.Name,
                Tier1Unit = tier?.Unit,
                Tier1ApplicationMethod = tier?.ApplicationMethod,

                // ✅ Balance Tier
                Tier1BalanceMinAmount = balance?.MinimumTierValue?.Amount,
                Tier1BalanceMinCurrency = balance?.MinimumTierValue?.Currency,
                Tier1BalanceMaxAmount = balance?.MaximumTierValue?.Amount,
                Tier1BalanceMaxCurrency = balance?.MaximumTierValue?.Currency,
                Tier1BalanceRate = balance?.TierRate,

                // ✅ LTV Tier
                Tier1LTVStart = ltv?.LTVStart,
                Tier1LTVEnd = ltv?.LTVEnd,
                Tier1LTVRate = ltv?.TierRate,

                // ✅ Rate Range
                Tier1RateRangeMin = tier?.RateRange?.MinimumRate,
                Tier1RateRangeMax = tier?.RateRange?.MaximumRate,
                Tier1RateRangeAdditionalInfo = tier?.RateRange?.AdditionalInformation,

                // ✅ Condition
                Condition1Field = condition?.Field,
                Condition1Operator = condition?.Operator,
                Condition1Value = condition?.Value,
                Condition1Description = condition?.Description,

                // Notes
                Notes = src.Notes,

                // ✅ Additional Info
                AdditionalInfo1Type = additional?.Type,
                AdditionalInfo1Description = additional?.Description,

                RateType = src.RateType
            };
        }


        //public static ICollection<OF.ProductData.Model.EFModel.Products.DepositRatesModel> MapDepositRates(
        // OF.ProductData.Model.CentralBank.Products.DepositRates src,
        // long paymentRequestId)
        //{
        //    if (src == null || src.RateDetails == null)
        //        return null;

        //    var result = new List<OF.ProductData.Model.EFModel.Products.DepositRatesModel>();

        //    var depositRate = new OF.ProductData.Model.EFModel.Products.DepositRatesModel
        //    {
        //        Id = Guid.NewGuid(),
        //        RequestId = paymentRequestId,

        //        // RateType
        //        RateType = src.RateType.ToString(),

        //        // RateDetails properties
        //        RateCategory = src.RateDetails.RateCategory.ToString(),
        //        AnnualRate = src.RateDetails.AnnualRate,

        //        // AnnualRateRange
        //        AnnualRateRangeMin = src.RateDetails.AnnualRateRange?.MinRate,
        //        AnnualRateRangeMax = src.RateDetails.AnnualRateRange?.MaxRate,

        //        // Tier
        //        TierMinBalance = src.RateDetails.Tier?.MinBalance,
        //        TierMaxBalance = src.RateDetails.Tier?.MaxBalance,
        //        TierCurrency = src.RateDetails.Tier?.Currency,


        //        // Term
        //        Term = src.RateDetails.Term,

        //        // Dates
        //        EffectiveDate = src.RateDetails.EffectiveDate,
        //        ExpiryDate = src.RateDetails.ExpiryDate,

        //        // Frequencies and methods
        //        CalculationMethod = src.RateDetails.CalculationMethod.ToString(),
        //        CalculationFrequency = src.RateDetails.CalculationFrequency.ToString(),
        //        ApplicationFrequency = src.RateDetails.ApplicationFrequency.ToString(),

        //        // Notes
        //        Notes = src.RateDetails.Notes
        //    };

        //    result.Add(depositRate);
        //    return result;
        //}

        //public static OF.ProductData.Model.EFModel.Products.FinanceRates MapFinanceInterestRate(
        // OF.ProductData.Model.CentralBank.Products.FinanceInterestRate src,
        // long paymentRequestId)
        //{
        //    if (src == null)
        //        return null;

        //    var financeRates = new OF.ProductData.Model.EFModel.Products.FinanceRates
        //    {
        //        Id = Guid.NewGuid(),
        //        RequestId = paymentRequestId,

        //        // Basic properties
        //        Description = src.Description,
        //        Rate = src.Rate,
        //        BenchMark = src.BenchMark,
        //        BenchMarkRate = src.BenchMarkRate,
        //        Margin = src.Margin,
        //        RateReviewFrequency = src.RateReviewFrequency,
        //        RateReviewNextDate = src.RateReviewNextDate,
        //        CalculationFrequency = src.CalculationFrequency,
        //        ApplicationFrequency = src.ApplicationFrequency,
        //        InterestCalculationMethod = src.InterestCalculationMethod,

        //        // AnnualPercentageRate
        //        AnnualPercentageRateStartingFrom =Convert.ToString( src.AnnualPercentageRate?.StartingFrom),
        //        AnnualPercentageRateUpTo = src.AnnualPercentageRate?.UpTo,
        //        AnnualPercentageRateAdditionalInfo = src.AnnualPercentageRate?.AdditionalInformation,

        //        // Notes
        //        Notes = src.Notes,

        //        // RateType
        //        RateType = src.RateType
        //    };

        //    // Map Tiers (take first tier if exists)
        //    if (src.Tiers != null && src.Tiers.Any())
        //    {
        //        var tier = src.Tiers[0];
        //        financeRates.Tier1Name = tier.Name;
        //        financeRates.Tier1Unit = tier.Unit;
        //        financeRates.Tier1ApplicationMethod = tier.ApplicationMethod;

        //        // Map Balance Tier Details (take first balance tier if exists)
        //        if (tier.BalanceTierDetails != null && tier.BalanceTierDetails.Any())
        //        {
        //            var balanceDetail = tier.BalanceTierDetails[0];
        //            financeRates.Tier1BalanceMinAmount = balanceDetail.MinimumTierValue?.Amount;
        //            financeRates.Tier1BalanceMinCurrency = balanceDetail.MinimumTierValue?.Currency;
        //            financeRates.Tier1BalanceMaxAmount = balanceDetail.MaximumTierValue?.Amount;
        //            financeRates.Tier1BalanceMaxCurrency = balanceDetail.MaximumTierValue?.Currency;
        //            financeRates.Tier1BalanceRate = balanceDetail.TierRate;
        //        }

        //        // Map LTV Tier Details (take first LTV tier if exists)
        //        if (tier.LTVTierDetails != null && tier.LTVTierDetails.Any())
        //        {
        //            var ltvDetail = tier.LTVTierDetails[0];
        //            financeRates.Tier1LTVStart = ltvDetail.LTVStart;
        //            financeRates.Tier1LTVEnd = ltvDetail.LTVEnd;
        //            financeRates.Tier1LTVRate = ltvDetail.TierRate;
        //        }

        //        // Map Rate Range
        //        if (tier.RateRange != null)
        //        {
        //            financeRates.Tier1RateRangeMin = tier.RateRange.MinimumRate;
        //            financeRates.Tier1RateRangeMax = tier.RateRange.MaximumRate;
        //            financeRates.Tier1RateRangeAdditionalInfo = tier.RateRange.AdditionalInformation;
        //        }
        //    }

        //    // Map Conditions (take first condition if exists)
        //    if (src.Conditions != null && src.Conditions.Any())
        //    {
        //        var condition = src.Conditions[0];
        //        financeRates.Condition1Field = condition.Field;
        //        financeRates.Condition1Operator = condition.Operator;
        //        financeRates.Condition1Value = condition.Value;
        //        financeRates.Condition1Description = condition.Description;
        //    }

        //    // Map Additional Information (take first additional info if exists)
        //    if (src.AdditionalInformation != null && src.AdditionalInformation.Any())
        //    {
        //        var additionalInfo = src.AdditionalInformation[0];
        //        financeRates.AdditionalInfo1Type = additionalInfo.Type;
        //        financeRates.AdditionalInfo1Description = additionalInfo.Description;
        //    }

        //    return financeRates;
        //}


        public static ICollection<OF.ProductData.Model.EFModel.Products.Tenor> MapTenor(ProductData.Model.CentralBank.Products.Tenor src, long paymentRequestId)
        {
            if (src == null)
                return null;

            return new List<OF.ProductData.Model.EFModel.Products.Tenor>
            {
                    new OF.ProductData.Model.EFModel.Products.Tenor
                    {
                        MinimumLoanCurrency = src.MinimumTenor,  // This is "P2Y3M" format
                        MaximumLoanCurrency = src.MaximumTenor,
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

                // Basic properties - convert enums to strings
                Type = src.Type.ToString(),
                AssetType = src.AssetType.ToString(),
                Description = src.Description
            };

            // Map Valuation (Valuation is an object, not a collection)
            if (src.Valuation != null)
            {
                assetBacked.ValuationDate = src.Valuation.Date;
                if (src.Valuation.Amount != null)
                {
                    assetBacked.ValuationAmount = Convert.ToDecimal(src.Valuation.Amount.Amount);
                    assetBacked.ValuationCurrency = src.Valuation.Amount.Currency;
                }
            }

            // Map SupplementaryInformation - store as JSON
            if (src.SupplementaryInformation != null)
            {
                assetBacked.SupplementaryInformationData = System.Text.Json.JsonSerializer.Serialize(src.SupplementaryInformation);
            }

            // Map OwnershipTransfer
            if (src.OwnershipTransfer != null)
            {
                assetBacked.TransferOfOwnershipDate = src.OwnershipTransfer.TransferOfOwnershipDate;
                assetBacked.OwnershipTransferType = src.OwnershipTransfer.Type.ToString();
                assetBacked.OwnershipTransferMethod = src.OwnershipTransfer.Method?.ToString();

                // Map TokenPurchaseAmount if it exists
                if (src.OwnershipTransfer.TokenPurchaseAmount != null)
                {
                    assetBacked.TokenPurchaseAmount = Convert.ToDecimal(src.OwnershipTransfer.TokenPurchaseAmount.Amount);
                    assetBacked.TokenPurchaseCurrency = src.OwnershipTransfer.TokenPurchaseAmount.Currency;
                }

                // Map BuyoutSchedule
                if (src.OwnershipTransfer.BuyoutSchedule != null)
                {
                    assetBacked.BuyoutScheduleFrequency = src.OwnershipTransfer.BuyoutSchedule.Frequency.ToString();
                    if (src.OwnershipTransfer.BuyoutSchedule.BuyoutAmount != null)
                    {
                        assetBacked.BuyoutScheduleAmount = Convert.ToDecimal(src.OwnershipTransfer.BuyoutSchedule.BuyoutAmount.Amount);
                        assetBacked.BuyoutScheduleCurrency = src.OwnershipTransfer.BuyoutSchedule.BuyoutAmount.Currency;
                    }
                }

                // Map SaleAgreement
                if (src.OwnershipTransfer.SaleAgreement != null)
                {
                    assetBacked.SaleAgreementRequired = src.OwnershipTransfer.SaleAgreement.Required;
                    assetBacked.SaleAgreementExecution = src.OwnershipTransfer.SaleAgreement.Execution.ToString();

                }
                if (src.OwnershipTransfer.Price != null)
                {
                    assetBacked.SaleAgreementPrice = Convert.ToDecimal(src.OwnershipTransfer.Price.Amount);
                    assetBacked.SaleAgreementPriceCurrency = src.OwnershipTransfer.Price.Currency;
                }
                // Map TransferConditions (convert List<string> to comma-separated string)
                if (src.OwnershipTransfer.TransferConditions != null && src.OwnershipTransfer.TransferConditions.Any())
                {
                    assetBacked.TransferConditions = string.Join(",", src.OwnershipTransfer.TransferConditions);
                }
            }

            return new List<OF.ProductData.Model.EFModel.Products.AssetBacked> { assetBacked };
        }
        public static ICollection<ProductData.Model.EFModel.Products.RewardsBenefits> MapRewardsBenefits(ProductData.Model.CentralBank.Products.RewardsBenefits src, long paymentRequestId)
        {
            if (src == null)
                return null;

            // Create a list and add the mapped object
            return new List<ProductData.Model.EFModel.Products.RewardsBenefits>
    {
        new ProductData.Model.EFModel.Products.RewardsBenefits
        {
            Name = src.Name,
            Description = src.Description,
            Type = src.Type.ToString(),

            BalanceAmount = src.Balance?.Amount != null
                ? Convert.ToDecimal(src.Balance.Amount)
                : (decimal?)null,

            BalanceCurrency = src.Balance?.Currency,

            RewardBasis = src.RewardBasis != null && src.RewardBasis.Any()
                ? string.Join(",", src.RewardBasis)
                : null,
            
            // FrequencyPaid
            FrequencyPaid = src.FrequencyPaid?.ToString()
        }
    };

        }
    }
}

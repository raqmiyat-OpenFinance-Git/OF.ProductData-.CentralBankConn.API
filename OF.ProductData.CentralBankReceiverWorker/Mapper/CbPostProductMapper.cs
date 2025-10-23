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
                        ProductCategory = prod.ProductCategory,
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
                        CurrentAccount = MapCurrentAccount(prod.Product?.CurrentAccount, paymentRequestId),
                        SavingsAccount = MapSavingsAccount(prod.Product?.SavingsAccount, paymentRequestId),
                        CreditCard = MapCreditCard(prod.Product?.CreditCard, paymentRequestId),
                        PersonalLoan = MapPersonalLoan(prod.Product?.PersonalLoan, paymentRequestId),
                        Mortgage = MapMortgage(prod.Product?.Mortgage, paymentRequestId),
                        ProfitSharingRate = MapProfitSharingRate(prod.Product?.ProfitSharingRate, paymentRequestId),
                        FinanceProfitRate = MapFinanceProfitRate(prod.Product?.FinanceProfitRate, paymentRequestId)
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


        public static ICollection<FinanceProfitRate> MapFinanceProfitRate(FinanceProfitRateData src, long paymentRequestId)
        {
            if (src == null) return null;

            var tier = src.Tiers?.FirstOrDefault(); // take first tier if exists
            var additionalInfo = src.AdditionalInformation?.FirstOrDefault();

            var entity = new FinanceProfitRate
            {
                RequestId = paymentRequestId,
                Name = src.Name,
                Description = src.Description,
                CalculationMethod = src.CalculationMethod,
                Rate = src.Rate,
                Frequency = src.Frequency,

                // Flatten Tier
                TiersType = tier?.Type,
                TiersDescription = tier?.Description,
                TiersName = tier?.Name,
                TiersUnit = tier?.Unit,
                TiersMinimumTierValue = tier?.MinimumTierValue?.AmountValue != null
                    ? decimal.Parse(tier.MinimumTierValue.AmountValue)
                    : (decimal?)null,
                TiersMinimumTierCurrency = tier?.MinimumTierValue.Currency,
                TiersMaximumTierValue = tier?.MaximumTierValue?.AmountValue != null
                    ? decimal.Parse(tier.MaximumTierValue.AmountValue)
                    : (decimal?)null,
                TiersMaximumTierCurrency = tier?.MaximumTierValue.Currency,
                TiersMinimumTierRate = tier?.MinimumTierRate,
                TiersMaximumTierRate = tier?.MaximumTierRate,
                TiersCondition = tier?.Condition,

                // Flatten Additional Information
                AdditionalInfoType = additionalInfo?.Type,
                AdditionalInfoDescription = additionalInfo?.Description
            };

            return new List<FinanceProfitRate> { entity };
        }

    }
}

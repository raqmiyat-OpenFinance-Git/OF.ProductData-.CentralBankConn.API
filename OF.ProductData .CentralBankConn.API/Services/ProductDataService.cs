using ConsentModel.Common;
using OF.ProductData.CentralBankConn.API.IServices;
using OF.ProductData.CentralBankConn.API.Repositories;
using OF.ProductData.Common.Custom;
using OF.ProductData.Common.Helpers;
using OF.ProductData.Common.NLog;
using OF.ProductData.Model.CentralBank;
using OF.ProductData.Model.CentralBank.Products;
using OF.ProductData.Model.Common;
using OF.ProductData.Model.CoreBank;
using OF.ProductData.Model.CoreBank.Products;
using Raqmiyat.Framework.Custom;
using System.Net.Http.Headers;

namespace OF.ProductData.CentralBankConn.API.Services;

public class ProductDataService : IProductDataService
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<CoreBankApis> _coreBankApis;
    private readonly IMasterRepository _masterRepository;
    private readonly IOptions<ApiHeaderParams> _apiHeaderParams;
    private readonly Custom _Custom;
    private readonly ProductLogger _logger;
    public ProductDataService(HttpClient httpClient, IOptions<CoreBankApis> coreBankApis, IMasterRepository masterRepository, IOptions<ApiHeaderParams> apiHeaderParams, Custom custom, ProductLogger logger)
    {
        _httpClient = httpClient;
        _coreBankApis = coreBankApis;
        _masterRepository = masterRepository;
        _apiHeaderParams = apiHeaderParams;
        _Custom = custom;
        _logger = logger;
    }

    public async Task<ApiResult<CbsProductResponse>> GetProductFromCoreBankAsync(CbsProductRequest cbProductRequest)
    {
        ApiResult<CbsProductResponse>? apiResult = null;

        try
        {

            _logger.Info($"GetProductFromCoreBankAsync is invoked");
            string jsonData = JsonConvert.SerializeObject(cbProductRequest, Formatting.Indented);

            _logger.Info($"CorrelationId: {cbProductRequest.CorrelationId} || JsonRequestBody: {PciDssSecurity.MaskCardInDynamicJson(jsonData, _logger.Log)}");



            var url = UrlHelper.CombineUrl(_coreBankApis.Value.BaseUrl!, _coreBankApis.Value.ProductServiceUrl!.GetProductUrl!);

            _logger.Info($"Request Url: {url}");

            if (string.IsNullOrWhiteSpace(url))
            {
                throw new InvalidOperationException("ProductServiceUrl.GetProductUrl is not configured.");
            }

            var content = GetStringContent(jsonData, cbProductRequest.CorrelationId.ToString());

            _httpClient.Timeout = TimeSpan.FromSeconds(30); // or pull from config
            _logger.Info($"CorrelationId: {cbProductRequest.CorrelationId} || Calling CoreBank API URL: {url}");


            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };
            var TokenapiUrl = _coreBankApis.Value.TokenUrl?.ToString();
            _logger.Info($"CorrelationId: {cbProductRequest?.CorrelationId} || Calling Token API URL: {TokenapiUrl}");
            var dynamicToken = await _Custom.PostTokenAsync(cbProductRequest?.CorrelationId.ToString()!, TokenapiUrl!, _apiHeaderParams, _logger.Log);
            request.Headers.Add("X-Correlation-ID", cbProductRequest?.CorrelationId.ToString());
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", dynamicToken.access_token);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage apiResponse = await _httpClient.SendAsync(request);


            _logger.Info($"StatusCode: {apiResponse.StatusCode}");

            string apiResponseBody = await apiResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            _logger.Info($"RawResponse: {apiResponseBody}");

            if (string.IsNullOrWhiteSpace(apiResponseBody))
            {
                throw new InvalidOperationException("CoreBank API returned empty response body.");
            }


            _logger.Info($"CorrelationId: {cbProductRequest!.CorrelationId} || Receiving Response data: {PciDssSecurity.MaskCardInDynamicJson(apiResponseBody, _logger.Log)}");

            if (apiResponse.IsSuccessStatusCode)
            {
                _logger.Info($"CorrelationId: {cbProductRequest!.CorrelationId} || API Response: SUCCESS");

                var json = JObject.Parse(apiResponseBody);
                var coreBankProductResponse = json["data"]?.ToObject<CbsProductResponse>() ?? throw new InvalidOperationException("Deserialized coreBankProductResponse is null.");
                apiResult = ApiResultFactory.Success(data: coreBankProductResponse, message: "Success", code: "200");
            }
            else
            {
                _logger.Warn($"CorrelationId: {cbProductRequest!.CorrelationId}|| ExternalRefNbr: {cbProductRequest.ExternalRefNbr} || API FAILED with status {(int)apiResponse.StatusCode} - {apiResponse.ReasonPhrase}");

                apiResult = ApiResultFactory.Failure<CbsProductResponse>(apiResponseBody, ((int)apiResponse.StatusCode).ToString());
            }

        }
        catch (HttpRequestException ex) when (ex.StatusCode.HasValue)
        {
            _logger.Error(ex,
            $"Method: GetProductFromCoreBankAsync || CorrelationId: {cbProductRequest.CorrelationId.ToString() ?? "N/A"} || " +
            $"HTTP error. StatusCode: {(int)(ex.StatusCode ?? 0)} - {ex.StatusCode} || Message: {ex.Message}");

            return ApiResultFactory.Failure<CbsProductResponse>("Internal server error", "502");
        }

        catch (Exception ex)
        {
            _logger.Error(ex, $"Unhandled exception: {ex.Message}");
            return ApiResultFactory.Failure<CbsProductResponse>("Internal server error", "500");
        }

        return apiResult!;
    }

    public CbProductDataResponse GetCentralBankProductByIdResponse(CbsProductResponse cbProductResponse, Logger logger)
    {
        var random = new Random();

        try
        {
            var response = new CbProductDataResponse
            {
                Data = new List<LFIData>
            {
                new LFIData
                {
                    LFIId = Guid.NewGuid().ToString(),
                    LFIBrandId = "Brand_" + random.Next(1000, 9999),
                    Products = new List<ProductWrapper>
                    {
                        new ProductWrapper
                        {
                            ProductId = "Prod_" + random.Next(10000, 99999),
                            ProductName = "Banking Products",
                            ProductCategory = Model.CentralBank.ProductCategory.SavingsAccount,
                            Description = "Dummy multi-product response",
                            EffectiveFromDateTime = DateTime.UtcNow.AddYears(-1),
                            EffectiveToDateTime = DateTime.UtcNow.AddYears(1),
                            LastUpdatedDateTime = DateTime.UtcNow,
                            IsShariaCompliant = false,
                            ShariaInformation = "Not Applicable",
                            IsSalaryTransferRequired = false,
                            Links = new Links
                            {
                                ApplicationUri = "http://example.com/apply",
                                ApplicationPhoneNumber = "+971500000000",
                                ApplicationEmail = "support@example.com",
                                ApplicationDescription = "Application info",
                                KfsUri = "http://example.com/kfs",
                                OverviewUri = "http://example.com/overview",
                                TermsUri = "http://example.com/terms",
                                FeesAndPricingUri = "http://example.com/fees",
                                ScheduleOfChargesUri = "http://example.com/schedule",
                                EligibilityUri = "http://example.com/eligibility",
                                CardImageUri = "http://example.com/card.png"
                            },
                            Eligibility = new EligibilityData
                            {
                                ResidenceStatus = new List<ResidenceStatusItem> { new ResidenceStatusItem { Type = ResidenceStatusType.UaeResident, Description = "A person who is a resident of the UAE." } },
                                EmploymentStatus = new List<EmploymentStatusItem> { new EmploymentStatusItem { Type = EmploymentStatusType.Salaried, Description = "Salaried employee" } },
                                CustomerType = new List<CustomerTypeItem> { new CustomerTypeItem { Type = CustomerType.Retail, Description = "Retail customer" } },
                                AccountOwnership = new List<AccountOwnershipItem> { new AccountOwnershipItem { Type = AccountOwnershipType.Individual, Description = "Individual ownership" } },
                                Age = new List<AgeEligibilityItem> { new AgeEligibilityItem { Type = AgeType.MinimumAge, Description = "Minimum 18 years", Value = 18 } },
                                AdditionalEligibility = new List<AdditionalEligibilityItem> { new AdditionalEligibilityItem { Type = AdditionalEligibilityType.Student, Description = "Student eligibility" } }
                            },
                            Channels = new List<Channel> { new Channel { Type = ChannelType.Phone, Description = "Apply via phone" } },
                            Product = new ProductDetails
                            {
                                CurrentAccount = new CurrentAccountData
                                {
                                    Type = CurrentAccountType.Basic,
                                    IsOverdraftAvailable = false,
                                    //MinimumBalance = new Amount { AmountValue = 1000, Currency = "AED" },
                                    Documentation = new List<Document>
                                    {
                                        new Document { Type = DocumentType.ApplicationForm, Description = "Fill application form" }
                                    },
                                    Features = new List<CurrentAccountFeature>
                                    {
                                        new CurrentAccountFeature
                                        {
                                            Type = FeatureTypeCurrentAccount.DebitCard,
                                            Description = "Debit card feature"
                                        }
                                    },
                                    Limits = new List<CurrentAccountLimit>
                                    {
                                        new CurrentAccountLimit
                                        {
                                            Type = LimitTypeCurrentAccount.MinimumDeposit,
                                            Description = "Minimum deposit",
                                            Value = 1000,
                                            Amount = new Amount { AmountValue = 1000, Currency = "AED" }
                                        }
                                    },
                                    Charges = new List<ProductCharge> { new ProductCharge() }
                                },
                                SavingsAccount = new SavingsAccountData
                                {
                                    Type = SavingsAccountType.Savings,
                                    MinimumBalance = new Amount { AmountValue = 1000, Currency = "AED" },
                                    Documentation = new List<Document> { new Document { Type = DocumentType.ApplicationForm, Description = "Fill application form" } },
                                    Features = new List<SavingsAccountFeature> { new SavingsAccountFeature { Type = FeatureTypeSavingsAccount.DebitCard, Description = "Debit card feature" } },
                                    Limits = new List<SavingsAccountLimit> { new SavingsAccountLimit { Type = LimitTypeSavingsAccount.MinimumOpeningBalance, Description = "Minimum opening balance", Value = 1000, Amount = new Amount { AmountValue = 1000, Currency = "AED" }}}
                                },
                                CreditCard = new CreditCardData
                                {
                                    Type = CreditCardType.Visa,
                                   
                                    Documentation = new List<Document>
                                    {
                                        new Document { Type = DocumentType.ApplicationForm, Description = "Fill application form" }
                                    },
                                    Features = new List<CreditCardFeature>
                                    {
                                        new CreditCardFeature
                                        {
                                            Type = FeatureTypeCreditCard.InternationalPayments,
                                            Description = "Make payments internationally with competitive exchange rates"
                                        }
                                    },
                                    Limits = new List<CreditCardLimit>
                                    {
                                        new CreditCardLimit
                                        {
                                            Type = LimitTypeCreditCard.MinimumCreditLimit,
                                            Description = "Minimum credit limit",
                                            Value = 5000,
                                            Amount = new Amount { AmountValue = 5000, Currency = "AED" }
                                        }
                                    }
                                },
                                Finance = new FinanceData
                                {
                                    Type = FinanceType.AutoFinance,
                                    MinimumFinanceAmount = new Amount { AmountValue = 50000, Currency = "AED" },
                                    MaximumFinanceAmount = new Amount { AmountValue = 500000, Currency = "AED" },
                                    Documentation = new List<Document>
                                    {
                                        new Document { Type = DocumentType.ApplicationForm, Description = "Fill application form" }
                                    },
                                    Features = new List<FinanceFeature>
                                    {
                                        new FinanceFeature
                                        {
                                            Type = FeatureTypeFinance.QuickApproval,
                                            Description = "Quick approval feature"
                                        }
                                    },
                                    Limits = new List<FinanceLimit>
                                    {
                                        new FinanceLimit
                                        {
                                            Type = LimitTypeFinance.MaximumOverpayment,
                                            Description = "Maximum overpayment limit",
                                            Value = 10000,
                                            Amount = new Amount { AmountValue = 10000, Currency = "AED" }
                                        }
                                    }
                                },
                                Mortgage = new MortgageData
                                {
                                    MinimumFinanceAmount = new Amount { AmountValue = 250000, Currency = "AED" },
                                    MaximumFinanceAmount = new Amount { AmountValue = 5000000, Currency = "AED" },
                                    DownPayment = new List<DownPaymentRequirement>
                                    {
                                        new DownPaymentRequirement
                                        {
                                            CustomerCategory = "UaeResident",
                                            MinimumPercent = 20,
                                            Basis = "Percentage of property value"
                                        }
                                    },
                                    Documentation = new List<Document>
                                    {
                                        new Document { Type = DocumentType.ApplicationForm, Description = "Fill application form" }
                                    },
                                    Features = new List<MortgageFeature>
                                    {
                                        new MortgageFeature
                                        {
                                            Type = FeatureTypeMortgage.PreApproval,
                                            Description = "Pre-approval feature"
                                        }
                                    },
                                    Limits = new List<MortgageLimit>
                                    {
                                        new MortgageLimit
                                        {
                                            Type = LimitTypeMortgage.MaximumOverpayment,
                                            Description = "Maximum overpayment limit",
                                            Value = 50000,
                                            Amount = new Amount { AmountValue = 50000, Currency = "AED" }
                                        }
                                    }
                                },
                               
                                Tenor = new List<Tenor>
                                {
                                    new Tenor
                                    {
                                        MinimumTenor = "P1Y",
                                        MaximumTenor = "P5Y",
                                        Condition = "Subject to bank approval"
                                    }
                                },
                                AssetBacked = new List<AssetBacked>
                                {
                                    new AssetBacked
                                    {
                                        Type = AssetBackedType.OwnershipTransfer,
                                        AssetType = AssetType.Property,
                                        Description = "Property pledged as collateral",
                                        Valuation = new List<Valuation>
                                        {
                                            new Valuation
                                            {
                                                Date = DateTime.UtcNow,
                                                Amount = new MoneyAmount
                                                {
                                                    Amount= "750000",
                                                    Currency = "AED"
                                                }
                                            }
                                        },
                                        SupplementaryInformation = new SupplementaryInformation
                                        {
                                            AdditionalData = "Valuation done by approved valuer"
                                        },
                                        OwnershipTransfer = new OwnershipTransfer
                                        {
                                            TransferOfOwnershipDate = DateTime.UtcNow.AddYears(20),
                                            Type = OwnershipTransferType.Gradual,
                                            Method = OwnershipTransferMethodType.Buyouts,
                                            BuyoutSchedule = new BuyoutSchedule
                                            {
                                                Frequency = PaymentFrequencyType.Monthly,
                                                BuyoutAmount = new MoneyAmount
                                                {
                                                    Amount = "2500",
                                                    Currency = "AED"
                                                }
                                            },
                                            TransferConditions = new List<TransferConditionType>
                                            {
                                                TransferConditionType.AllLeasePaymentsCompleted
                                            }
                                        }
                                    }
                                },
                                RewardsBenefits = new List<RewardsBenefits>
                                {
                                    new RewardsBenefits
                                    {
                                        Name = "Monthly Cashback Program",
                                        Description = "Earn cashback on eligible transactions",
                                        Type = RewardBenefitType.Cashback,
                                    }
                                },
                                

                               
                            },
                            DenominationCurrency="AED"

                        }
                    }
                }
            }
            };
            response.Meta = new LFIMeta
            {
                TotalPages = 1,
                TotalRecords = 1
            };
            return response;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error occurred in GetCentralBankProductByIdResponse()");
            throw;
        }
    }

    public CbProductDataResponse ResponseProductDetails(CbsProductResponse cbProductResponse, string category, Logger logger)
    {
        var random = new Random();
        var response = new CbProductDataResponse();

        try
        {
            var lfiDataList = new List<LFIData>();
            var lFIData = new LFIData
            {
                LFIId = Guid.NewGuid().ToString(),
                LFIBrandId = Guid.NewGuid().ToString(),
                Products = new List<ProductWrapper>()
            };

            logger.Info($"Category: {category}");

            // Helper functions
            string RandomAmount(int min, int max) => random.Next(min, max).ToString("N2");
            decimal RandomRate(decimal min, decimal max) => Math.Round((decimal)(random.NextDouble() * ((double)max - (double)min) + (double)min), 2);

            string suffix = random.Next(100, 999).ToString();
            var effectiveFrom = DateTime.UtcNow.AddDays(-random.Next(10, 300));
            var effectiveTo = effectiveFrom.AddMonths(random.Next(12, 36));

            // Common Links
            var commonLinks = new Links
            {
                ApplicationUri = $"https://bank.example.com/apply/{category?.ToLower()}/{suffix}",
                ApplicationPhoneNumber = $"+9716005{random.Next(10000, 99999)}",
                ApplicationEmail = $"{category?.ToLower()}{suffix}@bank.example.com",
                ApplicationDescription = "Apply online or visit any branch",
                KfsUri = $"https://bank.example.com/kfs/{category?.ToLower()}/{suffix}",
                OverviewUri = $"https://bank.example.com/{category?.ToLower()}/{suffix}/overview",
                FeesAndPricingUri = $"https://bank.example.com/{category?.ToLower()}/{suffix}/fees",
                TermsUri = $"https://bank.example.com/{category?.ToLower()}/{suffix}/terms",
                ScheduleOfChargesUri = $"https://bank.example.com/{category?.ToLower()}/{suffix}/schedule",
                EligibilityUri = $"https://bank.example.com/{category?.ToLower()}/{suffix}/eligibility",
                CardImageUri = $"https://bank.example.com/{category?.ToLower()}/{suffix}/card.png"
            };

            // Common Eligibility
            var commonEligibility = new EligibilityData
            {
                ResidenceStatus = new List<ResidenceStatusItem> { new ResidenceStatusItem { Type = ResidenceStatusType.UaeResident, Description = "UAE Resident" } },
                EmploymentStatus = new List<EmploymentStatusItem> { new EmploymentStatusItem { Type = EmploymentStatusType.Salaried, Description = "Salaried Employee" } },
                CustomerType = new List<CustomerTypeItem> { new CustomerTypeItem { Type = CustomerType.Retail, Description = "Individual" } },
                AccountOwnership = new List<AccountOwnershipItem> { new AccountOwnershipItem { Type = AccountOwnershipType.Individual, Description = "Single Account" } },
                Age = new List<AgeEligibilityItem> { new AgeEligibilityItem { Type = AgeType.MinimumAge, Description = "Minimum age requirement", Value = 18 } },
                FinancialRequirements = new List<FinancialRequirementItem> { new FinancialRequirementItem { Type = FinancialRequirementType.MinimumDisposableIncome, Description = "Minimum monthly salary required", Value = 3000, Amount = new Amount { AmountValue = 3000, Currency = "AED" } } },
                AdditionalEligibility = new List<AdditionalEligibilityItem> { new AdditionalEligibilityItem { Type = AdditionalEligibilityType.Other, Description = "Valid ID Proof Required" } }
            };

            // Common Documentation
            var commonDocumentation = new List<Document> { new Document { Type = DocumentType.ApplicationForm, Description = "Fill application form" } };

            // Base Product Wrapper
            var product = new ProductWrapper
            {
                EffectiveFromDateTime = effectiveFrom,
                EffectiveToDateTime = effectiveTo,
                LastUpdatedDateTime = DateTime.UtcNow,
                IsShariaCompliant = random.Next(0, 2) == 1,
                ShariaInformation = "This financing product is structured under Murabaha principles. The bank purchases the asset and sells it to the customer at a pre-agreed profit margin. No interest is involved.",
                Links = commonLinks,
                Eligibility = commonEligibility,
                DenominationCurrency = "AED",
            };
           

            var productDetails = new ProductDetails();

            switch (category?.ToUpper())
            {

                case "CURRENTACCOUNT":
                    product.ProductCategory = Model.CentralBank.ProductCategory.CurrentAccount;
                    product.ProductId = $"CUR{suffix}";
                    product.ProductName = $"CurrentAccount";
                    product.Description = "Ideal for small and medium enterprises for day-to-day transactions..";
                    product.Channels = new List<Channel> { new Channel { Type = ChannelType.Phone, Description = "Apply via phone" }, new Channel { Type = ChannelType.Branch, Description = "Visit branch" } };

                    productDetails.CurrentAccount = new CurrentAccountData
                    {
                        Type = CurrentAccountType.Basic,
                        IsOverdraftAvailable = false,
                        //MinimumBalance = new Amount { AmountValue = 1000, Currency = "AED" },
                        Documentation = commonDocumentation,
                        Features = new List<CurrentAccountFeature> { new CurrentAccountFeature { Type = FeatureTypeCurrentAccount.DebitCard, Description = "Debit card feature" } },
                        Charges = new List<ProductCharge> { new ProductCharge { Type = ChargeType.MonthlyFees, Name = "ATM Withdrawal Fee", Description = "Fee charged for ATM withdrawals beyond free limit", Charge = new List<ChargeComponent> { new ChargeComponent { Amount = new Amount { AmountValue = 2.50m, Currency = "AED" }, MaximumChargeAmount = new Amount { AmountValue = 20000.0m, Currency = "AED" }, Rate = 0, ApplicationFrequency = "Per Transaction", InterestCalculationMethod = "Flat", Basis = "Per Withdrawal" } }, Conditions = new List<Condition> { new Condition { Field = "WithdrawalCount", Operator = ">", Value = "5", Description = "Applicable after 5 free withdrawals per month" } }, Justification = "Covers operational ATM costs", Frequency = "Per Transaction", DonatedToCharity = false, Notes = "First 5 transactions are free", SupplementaryInformation = new { Info = "Applicable only for other bank ATMs" } } },
                        Limits = new List<CurrentAccountLimit> { new CurrentAccountLimit { Type = LimitTypeCurrentAccount.MinimumBalance, Description = "Minimum balance", Value = 1000, Amount = new Amount { AmountValue = 1000, Currency = "AED" } } }
                    };

                    productDetails.DepositRates = new DepositRatesData
                    {
                        RateType = RateType.FixedInterest,
                        RateDetails = new List<RateDetail> { new RateDetail { RateCategory = RateCategoryType.Standard, AnnualRate = 0.5m, AnnualRateRange = new AnnualRateRange
                        {  MinRate = 0.5m, MaxRate = 0.5m  },Tier = new TierDetail{  MinBalance= "1000",MaxBalance = "50000",Currency = "AED"},Term = "P2Y3M",
                            EffectiveDate = new DateTime(1970, 1, 1),
                            ExpiryDate = new DateTime(1970, 1, 1), CalculationMethod = CalculationMethodType.AverageDailyBalance, CalculationFrequency = CalculationFrequencyType.Monthly, ApplicationFrequency = ApplicationFrequencyType.Monthly, Notes = "Competitive interest rate" } }
                    };
                    productDetails.RewardsBenefits = new List<RewardsBenefits> { new RewardsBenefits { Name = "Cashback Rewards", Description = "Cashback rewards on every transaction", Type = RewardBenefitType.Cashback, RewardBasis = new List<string> { "Card spend" }} };
                    break;

                case "SAVINGSACCOUNT":
                    product.ProductCategory = Model.CentralBank.ProductCategory.SavingsAccount;
                    product.ProductId = $"SAV{suffix}";
                    product.ProductName = $"SavingsAccount";
                    product.Description = "High-return savings account with easy fund access.";
                    product.Channels = new List<Channel> { new Channel { Type = ChannelType.MobileApp, Description = "Apply via app" }, new Channel { Type = ChannelType.Branch, Description = "Visit branch" } };

                    productDetails.SavingsAccount = new SavingsAccountData
                    {
                        Type = SavingsAccountType.Savings,
                        MinimumBalance = new Amount { AmountValue = 1000, Currency = "AED" },
                        Documentation = commonDocumentation,
                        Features = new List<SavingsAccountFeature> { new SavingsAccountFeature { Type = FeatureTypeSavingsAccount.SMSNotifications, Description = "SMS alerts" } },
                        Charges = new List<ProductCharge> { new ProductCharge { Type = ChargeType.MonthlyFees, Name = "ATM Withdrawal Fee", Description = "Fee charged for ATM withdrawals beyond free limit", Charge = new List<ChargeComponent> { new ChargeComponent { Amount = new Amount { AmountValue = 2.50m, Currency = "AED" }, MaximumChargeAmount = new Amount { AmountValue = 20000.0m, Currency = "AED" }, Rate = 0, ApplicationFrequency = "Per Transaction", InterestCalculationMethod = "Flat", Basis = "Per Withdrawal" } }, Conditions = new List<Condition> { new Condition { Field = "WithdrawalCount", Operator = ">", Value = "5", Description = "Applicable after 5 free withdrawals per month" } }, Justification = "Covers operational ATM costs", Frequency = "Per Transaction", DonatedToCharity = false, Notes = "First 5 transactions are free", SupplementaryInformation = new { Info = "Applicable only for other bank ATMs" } } },
                        Limits = new List<SavingsAccountLimit> { new SavingsAccountLimit { Type = LimitTypeSavingsAccount.MinimumOpeningBalance, Description = "Minimum opening balance", Value = 1000, Amount = new Amount { AmountValue = 1000, Currency = "AED" } } }
                    };

                    // Applicable optional sections for SavingsAccount
                    productDetails.DepositRates = new DepositRatesData
                    {
                        RateType = RateType.FixedInterest,
                        RateDetails = new List<RateDetail> { new RateDetail { RateCategory = RateCategoryType.Standard, AnnualRate = 0.5m, AnnualRateRange = new AnnualRateRange
                        {  MinRate = 0.5m, MaxRate = 0.5m  },Tier = new TierDetail{  MinBalance= "1000",MaxBalance = "50000",Currency = "AED"},Term = "P2Y3M",
                            EffectiveDate = new DateTime(1970, 1, 1),
                            ExpiryDate = new DateTime(1970, 1, 1), CalculationMethod = CalculationMethodType.AverageDailyBalance, CalculationFrequency = CalculationFrequencyType.Monthly, ApplicationFrequency = ApplicationFrequencyType.Monthly, Notes = "Competitive interest rate" } }
                    };
                    productDetails.RewardsBenefits = new List<RewardsBenefits> { new RewardsBenefits { Name = "Welcome Bonus", Description = "Cashback rewards on every transaction", Type = RewardBenefitType.Cashback, RewardBasis = new List<string> { "First deposit" }} };
                    break;

                case "CREDITCARD":
                    product.ProductCategory = Model.CentralBank.ProductCategory.CreditCard;
                    product.ProductId = $"CC{suffix}";
                    product.ProductName = $"CreditCard";
                    product.Description = "Premium credit card with rewards.";
                    product.IsSalaryTransferRequired = random.Next(0, 2) == 1;
                    product.Channels = new List<Channel> { new Channel { Type = ChannelType.MobileApp, Description = "Apply via app" }, new Channel { Type = ChannelType.Internet, Description = "Apply online" } };

                    productDetails.CreditCard = new CreditCardData
                    {
                        Type = CreditCardType.Visa,
                        Documentation = commonDocumentation,
                        Features = new List<CreditCardFeature> { new CreditCardFeature { Type = FeatureTypeCreditCard.InternationalPayments, Description = "International payments" } },
                        Charges = new List<ProductCharge> { new ProductCharge { Type = ChargeType.MonthlyFees, Name = "ATM Withdrawal Fee", Description = "Fee charged for ATM withdrawals beyond free limit", Charge = new List<ChargeComponent> { new ChargeComponent { Amount = new Amount { AmountValue = 2.50m, Currency = "AED" }, MaximumChargeAmount = new Amount { AmountValue = 20000.0m, Currency = "AED" }, Rate = 2, ApplicationFrequency = "Per Transaction", InterestCalculationMethod = "Flat", Basis = "Per Withdrawal" } }, Conditions = new List<Condition> { new Condition { Field = "WithdrawalCount", Operator = ">", Value = "5", Description = "Applicable after 5 free withdrawals per month" } }, Justification = "Covers operational ATM costs", Frequency = "Per Transaction", DonatedToCharity = false, Notes = "First 5 transactions are free", SupplementaryInformation = new { Info = "Applicable only for other bank ATMs" } } },
                        Limits = new List<CreditCardLimit> { new CreditCardLimit { Type = LimitTypeCreditCard.MinimumCreditLimit, Description = "Minimum credit limit", Value = 5000, Amount = new Amount { AmountValue = 5000, Currency = "AED" } } }
                    };

                    // Applicable optional sections for CreditCard
                    productDetails.FinanceRates = new FinanceRatesData
                    {
                        RateType = RateType.FixedInterest.ToString(),

                        RateOption = new List<RateOption>
                        {
                           new RateOption
                    {
                    RateType = RateType.FixedInterest,
                         AnnualPercentageRate = new AnnualPercentageRate
                        {
                            StartingFrom = "12.0",
                            UpTo = 15.0m,
                            AdditionalInformation = "Fixed period APR"
                        },

                        Tiers = new List<Tier>
                        {
                            new Tier
                            {
                                Name = "Fixed Tier",
                                Unit = TierUnitType.Balance,
                                ApplicationMethod = ApplicationMethodType.PerTier,

                                BalanceTierDetails = new List<BalanceTierDetail>
                                {
                                    new BalanceTierDetail
                                    {
                                        MinimumTierValue = new MoneyAmount { Amount = "10", Currency = "AED" },
                                        MaximumTierValue = new MoneyAmount { Amount = "500000", Currency = "AED" },
                                        TierRate = 13.5m
                                    }
                                },
                                LTVTierDetails=new List<LTVTierDetail>
                                {
                                    new LTVTierDetail
                                    {
                                        LTVStart=0.5m,
                                        LTVEnd=0.5m,
                                        TierRate=0.5m
                                    }
                                },
                                RateRange = new RateRange()
                                {
                                    MinimumRate=0.5m,
                                    MaximumRate=0.5m,
                                     AdditionalInformation = "Fixed promotional rate for selected customers"
                                }
                            }
                        },
                   // ✅ FIXED RATE
                    FixedRate = new FixedRateData
                    {
                        Description = "Fixed interest for initial period",
                        Rate = RandomRate(12.0m, 15.0m),
                        FixedRateEndDate = DateTime.UtcNow.AddYears(2),

                        CalculationFrequency = "Monthly",
                        ApplicationFrequency = "Monthly",
                        ProfitCalculationMethod = InterestCalculationMethodType.OutstandingBalance.ToString(),

                        AnnualPercentageRate = new AnnualPercentageRate
                        {
                            StartingFrom = "12.0",
                            UpTo = 15.0m,
                            AdditionalInformation = "Fixed period APR"
                        },

                        Tiers = new List<Tier>
                        {
                            new Tier
                            {
                                Name = "Fixed Tier",
                                Unit = TierUnitType.Balance,
                                ApplicationMethod = ApplicationMethodType.PerTier,

                                BalanceTierDetails = new List<BalanceTierDetail>
                                {
                                    new BalanceTierDetail
                                    {
                                        MinimumTierValue = new MoneyAmount { Amount = "10", Currency = "AED" },
                                        MaximumTierValue = new MoneyAmount { Amount = "500000", Currency = "AED" },
                                        TierRate = 13.5m
                                    }
                                },
                                LTVTierDetails=new List<LTVTierDetail>
                                {
                                    new LTVTierDetail
                                    {
                                        LTVStart=0.5m,
                                        LTVEnd=0.5m,
                                        TierRate=0.5m
                                    }
                                },
                                RateRange = new RateRange()
                                {
                                    MinimumRate=0.5m,
                                    MaximumRate=0.5m,
                                     AdditionalInformation = "Fixed promotional rate for selected customers"
                                }
                            }
                        },
                        FixedRateEnd= "2028-12-31",
                    },

                    // ✅ VARIABLE RATE
                    VariableRate = new VariableRateData
                    {
                        Description = "Variable interest after fixed period",
                        Rate = RandomRate(15.0m, 22.0m),
                        BenchMark = "EIBOR",
                        BenchMarkRate = RandomRate(10.0m, 15.0m),
                        Margin = RandomRate(2.0m, 4.0m),

                        RateReviewFrequency = "P6M",
                        RateReviewNextDate = DateTime.UtcNow.AddMonths(6),

                        CalculationFrequency = "Monthly",
                        ApplicationFrequency = "Monthly",
                        ProfitCalculationMethod = InterestCalculationMethodType.OutstandingBalance.ToString(),

                        AnnualPercentageRate = new AnnualPercentageRate
                        {
                            StartingFrom = "12.0",
                            UpTo = 22.0m,
                            AdditionalInformation = "Variable APR"
                        },

                        Tiers = new List<Tier>
                        {
                            new Tier
                            {
                                Name = "Variable Tier",
                                Unit = TierUnitType.Balance,
                                ApplicationMethod = ApplicationMethodType.PerTier,

                                BalanceTierDetails = new List<BalanceTierDetail>
                                {
                                    new BalanceTierDetail
                                    {
                                        MinimumTierValue = new MoneyAmount { Amount= "10", Currency = "AED" },
                                        MaximumTierValue = new MoneyAmount { Amount = "500000", Currency = "AED" },
                                        TierRate = 18.5m
                                    }
                                },
                                 LTVTierDetails=new List<LTVTierDetail>
                                {
                                    new LTVTierDetail
                                    {
                                        LTVStart=0.5m,
                                        LTVEnd=0.5m,
                                        TierRate=0.5m
                                    }
                                },
                                RateRange = new RateRange()
                                {
                                    MinimumRate=0.5m,
                                    MaximumRate=0.5m,
                                     AdditionalInformation = "Fixed promotional rate for selected customers"
                                }
                            }
                        },

                        VariableTerm = "P2Y3M"
                    },

                    // ✅ COMMON
                    Conditions = new List<Condition>
                    {
                        new Condition
                        {
                            Field = "CreditScore",
                            Operator = ">=",
                            Value = "700",
                            Description = "Better rates for high credit score"
                        }
                    },
                    Notes="string",
                    AdditionalInformation = new List<AdditionalInformationItem>
                    {
                        new AdditionalInformationItem
                        {
                            Type = AdditionalInformationType.Other,
                            Description = "Hybrid loan: fixed + variable"
                        }
                    }
        }
    }
                    };
                    productDetails.Tenor = new List<Tenor> { new Tenor { MinimumTenor = "P1M", MaximumTenor = "P12M", Condition = "Instalment plans available" } };
                    productDetails.RewardsBenefits = new List<RewardsBenefits> { new RewardsBenefits { Name = "Reward Points", Description = "Cashback rewards on every transaction", Type = RewardBenefitType.Points, RewardBasis = new List<string> { "Every AED 1 spent" } } };
                    break;

                case "FINANCE":
                    product.ProductCategory = Model.CentralBank.ProductCategory.Finance;
                    product.ProductId = $"FIN{suffix}";
                    product.ProductName = $"Finance";
                    product.Description = "Flexible personal finance solution.";
                    product.Channels = new List<Channel> { new Channel { Type = ChannelType.Phone, Description = "Call to apply" }, new Channel { Type = ChannelType.RelationshipManager, Description = "Contact RM" } };

                    productDetails.Finance = new FinanceData
                    {
                        Type = FinanceType.Finance,
                        MinimumFinanceAmount = new Amount { AmountValue = 10000, Currency = "AED" },
                        MaximumFinanceAmount = new Amount { AmountValue = 250000, Currency = "AED" },
                        Documentation = commonDocumentation,
                        Features = new List<FinanceFeature> { new FinanceFeature { Type = FeatureTypeFinance.FlexibleRepaymentPeriods, Description = "Flexible repayments" } },
                        Charges = new List<ProductCharge> { new ProductCharge { Type = ChargeType.MonthlyFees, Name = "ATM Withdrawal Fee", Description = "Fee charged for ATM withdrawals beyond free limit", Charge = new List<ChargeComponent> { new ChargeComponent { Amount = new Amount { AmountValue = 2.50m, Currency = "AED" }, MaximumChargeAmount = new Amount { AmountValue = 20000.0m, Currency = "AED" }, Rate = 0, ApplicationFrequency = "Per Transaction", InterestCalculationMethod = "Flat", Basis = "Per Withdrawal" } }, Conditions = new List<Condition> { new Condition { Field = "WithdrawalCount", Operator = ">", Value = "5", Description = "Applicable after 5 free withdrawals per month" } }, Justification = "Covers operational ATM costs", Frequency = "Per Transaction", DonatedToCharity = false, Notes = "First 5 transactions are free", SupplementaryInformation = new { Info = "Applicable only for other bank ATMs" } } },
                        Limits = new List<FinanceLimit> { new FinanceLimit { Type = LimitTypeFinance.MaximumOverpayment, Description = "Max overpayment", Value = 5000, Amount = new Amount { AmountValue = 5000, Currency = "AED" } } }
                    };

                    // Applicable optional sections for Finance
                    productDetails.FinanceRates = new FinanceRatesData
                    {
                        RateType = RateType.FixedInterest.ToString(),

                        RateOption = new List<RateOption>
                        {
                           new RateOption
                    {
                    RateType = RateType.FixedInterest,
                         AnnualPercentageRate = new AnnualPercentageRate
                        {
                            StartingFrom = "12.0",
                            UpTo = 15.0m,
                            AdditionalInformation = "Fixed period APR"
                        },

                        Tiers = new List<Tier>
                        {
                            new Tier
                            {
                                Name = "Fixed Tier",
                                Unit = TierUnitType.Balance,
                                ApplicationMethod = ApplicationMethodType.PerTier,

                                BalanceTierDetails = new List<BalanceTierDetail>
                                {
                                    new BalanceTierDetail
                                    {
                                        MinimumTierValue = new MoneyAmount { Amount = "10", Currency = "AED" },
                                        MaximumTierValue = new MoneyAmount { Amount = "500000", Currency = "AED" },
                                        TierRate = 13.5m
                                    }
                                },
                                LTVTierDetails=new List<LTVTierDetail>
                                {
                                    new LTVTierDetail
                                    {
                                        LTVStart=0.5m,
                                        LTVEnd=0.5m,
                                        TierRate=0.5m
                                    }
                                },
                                RateRange = new RateRange()
                                {
                                    MinimumRate=0.5m,
                                    MaximumRate=0.5m,
                                     AdditionalInformation = "Fixed promotional rate for selected customers"
                                }
                            }
                        },
                   // ✅ FIXED RATE
                    FixedRate = new FixedRateData
                    {
                        Description = "Fixed interest for initial period",
                        Rate = RandomRate(12.0m, 15.0m),
                        FixedRateEndDate = DateTime.UtcNow.AddYears(2),

                        CalculationFrequency = "Monthly",
                        ApplicationFrequency = "Monthly",
                        ProfitCalculationMethod = InterestCalculationMethodType.OutstandingBalance.ToString(),

                        AnnualPercentageRate = new AnnualPercentageRate
                        {
                            StartingFrom = "12.0",
                            UpTo = 15.0m,
                            AdditionalInformation = "Fixed period APR"
                        },

                        Tiers = new List<Tier>
                        {
                            new Tier
                            {
                                Name = "Fixed Tier",
                                Unit = TierUnitType.Balance,
                                ApplicationMethod = ApplicationMethodType.PerTier,

                                BalanceTierDetails = new List<BalanceTierDetail>
                                {
                                    new BalanceTierDetail
                                    {
                                        MinimumTierValue = new MoneyAmount { Amount= "10", Currency = "AED" },
                                        MaximumTierValue = new MoneyAmount { Amount = "500000", Currency = "AED" },
                                        TierRate = 13.5m
                                    }
                                },
                                LTVTierDetails=new List<LTVTierDetail>
                                {
                                    new LTVTierDetail
                                    {
                                        LTVStart=0.5m,
                                        LTVEnd=0.5m,
                                        TierRate=0.5m
                                    }
                                },
                                RateRange = new RateRange()
                                {
                                    MinimumRate=0.5m,
                                    MaximumRate=0.5m,
                                     AdditionalInformation = "Fixed promotional rate for selected customers"
                                }
                            }
                        },
                        FixedRateEnd= "2028-12-31",
                    },

                    // ✅ VARIABLE RATE
                    VariableRate = new VariableRateData
                    {
                        Description = "Variable interest after fixed period",
                        Rate = RandomRate(15.0m, 22.0m),
                        BenchMark = "EIBOR",
                        BenchMarkRate = RandomRate(10.0m, 15.0m),
                        Margin = RandomRate(2.0m, 4.0m),

                        RateReviewFrequency = "P6M",
                        RateReviewNextDate = DateTime.UtcNow.AddMonths(6),

                        CalculationFrequency = "Monthly",
                        ApplicationFrequency = "Monthly",
                        ProfitCalculationMethod = InterestCalculationMethodType.OutstandingBalance.ToString(),

                        AnnualPercentageRate = new AnnualPercentageRate
                        {
                            StartingFrom = "12.0",
                            UpTo = 22.0m,
                            AdditionalInformation = "Variable APR"
                        },

                        Tiers = new List<Tier>
                        {
                            new Tier
                            {
                                Name = "Variable Tier",
                                Unit = TierUnitType.Balance,
                                ApplicationMethod = ApplicationMethodType.PerTier,

                                BalanceTierDetails = new List<BalanceTierDetail>
                                {
                                    new BalanceTierDetail
                                    {
                                        MinimumTierValue = new MoneyAmount { Amount = "10", Currency = "AED" },
                                        MaximumTierValue = new MoneyAmount { Amount = "500000", Currency = "AED" },
                                        TierRate = 18.5m
                                    }
                                },
                                 LTVTierDetails=new List<LTVTierDetail>
                                {
                                    new LTVTierDetail
                                    {
                                        LTVStart=0.5m,
                                        LTVEnd=0.5m,
                                        TierRate=0.5m
                                    }
                                },
                                RateRange = new RateRange()
                                {
                                    MinimumRate=0.5m,
                                    MaximumRate=0.5m,
                                     AdditionalInformation = "Fixed promotional rate for selected customers"
                                }
                            }
                        },

                        VariableTerm = "P2Y3M"
                    },

                        // ✅ COMMON
                        Conditions = new List<Condition>
                        {
                            new Condition
                            {
                                Field = "CreditScore",
                                Operator = ">=",
                                Value = "700",
                                Description = "Better rates for high credit score"
                            }
                        },
                        Notes="string",
                        AdditionalInformation = new List<AdditionalInformationItem>
                        {
                            new AdditionalInformationItem
                            {
                                Type = AdditionalInformationType.Other,
                                Description = "Hybrid loan: fixed + variable"
                            }
                        }
        }
    }
                    };
                    productDetails.Tenor = new List<Tenor> { new Tenor { MinimumTenor = "P1Y", MaximumTenor = "P5Y", Condition = "Flexible repayment terms" } };
                    productDetails.AssetBacked = new List<AssetBacked>
                    {
                            new AssetBacked
                            {
                                Type = AssetBackedType.Collateral,
                                AssetType = AssetType.Property,
                                Description = "string",

                                Valuation = new List<Valuation>
                                {
                                    new Valuation
                                    {
                                        Date = new DateTime(1970, 1, 1),
                                        Amount = new MoneyAmount
                                        {
                                            Amount= "100000", // replace with actual value
                                            Currency = "AED"
                                        }
                                    }
                                },

                                SupplementaryInformation = new Dictionary<string, object>(),

                                OwnershipTransfer = new OwnershipTransfer
                                {
                                    TransferOfOwnershipDate = new DateTime(1970, 1, 1),
                                    Type = OwnershipTransferType.Gift,
                                    Method = OwnershipTransferMethodType.EndOfLease,

                                    TokenPurchaseAmount = new MoneyAmount
                                    {
                                        Amount = "5000",
                                        Currency = "AED"
                                    },

                                    BuyoutSchedule = new BuyoutSchedule
                                    {
                                        Frequency = PaymentFrequencyType.Weekly,
                                        BuyoutAmount = new MoneyAmount
                                        {
                                            Amount = "1000",
                                            Currency = "AED"
                                        }
                                    },

                                    SaleAgreement = new SaleAgreement
                                    {
                                        Required = false,
                                        Execution =SaleAgreementExecutionType.AtLeaseCompletion,
                                        Price = new MoneyAmount
                                        {
                                            Amount= "120000",
                                            Currency = "AED"
                                        }
                                    },

                                    TransferConditions = new List<TransferConditionType>
                                    {
                                        TransferConditionType.AllLeasePaymentsCompleted
                                    }
                                }
                            }
                    };
                    break;

                case "MORTGAGE":
                    product.ProductCategory = Model.CentralBank.ProductCategory.Mortgage;
                    product.ProductId = $"MTG{suffix}";
                    product.ProductName = $"Mortgage";
                    product.Description = "Competitive home mortgage.";
                    product.Channels = new List<Channel> { new Channel { Type = ChannelType.RelationshipManager, Description = "Contact specialist" }, new Channel { Type = ChannelType.Branch, Description = "Visit branch" } };

                    productDetails.Mortgage = new MortgageData
                    {
                        MinimumFinanceAmount = new Amount { AmountValue = 250000, Currency = "AED" },
                        MaximumFinanceAmount = new Amount { AmountValue = 5000000, Currency = "AED" },
                        DownPayment = new List<DownPaymentRequirement> { new DownPaymentRequirement { CustomerCategory = "UaeResident", MinimumPercent = 20, Basis = "Percentage of property value" } },
                        Documentation = commonDocumentation,
                        Features = new List<MortgageFeature> { new MortgageFeature { Type = FeatureTypeMortgage.PreApproval, Description = "Pre-approval available" } },
                        Charges = new List<ProductCharge> { new ProductCharge { Type = ChargeType.MonthlyFees, Name = "ATM Withdrawal Fee", Description = "Fee charged for ATM withdrawals beyond free limit", Charge = new List<ChargeComponent> { new ChargeComponent { Amount = new Amount { AmountValue = 2.50m, Currency = "AED" }, MaximumChargeAmount = new Amount { AmountValue = 20000.0m, Currency = "AED" }, Rate = 0, ApplicationFrequency = "Per Transaction", InterestCalculationMethod = "Flat", Basis = "Per Withdrawal" } }, Conditions = new List<Condition> { new Condition { Field = "WithdrawalCount", Operator = ">", Value = "5", Description = "Applicable after 5 free withdrawals per month" } }, Justification = "Covers operational ATM costs", Frequency = "Per Transaction", DonatedToCharity = false, Notes = "First 5 transactions are free", SupplementaryInformation = new { Info = "Applicable only for other bank ATMs" } } },
                        Limits = new List<MortgageLimit> { new MortgageLimit { Type = LimitTypeMortgage.MaximumOverpayment,
                        Description = "Max overpayment", Value = 50000,
                        Amount = new Amount { AmountValue = 50000, Currency = "AED" },Percentage=0.5m} }
                    };

                    // Applicable optional sections for Mortgage
                    productDetails.FinanceRates = new FinanceRatesData
                    {
                        RateType = RateType.FixedInterest.ToString(),

                        RateOption = new List<RateOption>
                        {
                           new RateOption
                    {
                    RateType = RateType.FixedInterest,
                         AnnualPercentageRate = new AnnualPercentageRate
                        {
                            StartingFrom = "12.0",
                            UpTo = 15.0m,
                            AdditionalInformation = "Fixed period APR"
                        },

                        Tiers = new List<Tier>
                        {
                            new Tier
                            {
                                Name = "Fixed Tier",
                                Unit = TierUnitType.Balance,
                                ApplicationMethod = ApplicationMethodType.PerTier,
                                BalanceTierDetails = new List<BalanceTierDetail>
                                {
                                    new BalanceTierDetail
                                    {
                                        MinimumTierValue = new MoneyAmount { Amount = "10", Currency = "AED" },
                                        MaximumTierValue = new MoneyAmount { Amount = "500000", Currency = "AED" },
                                        TierRate = 13.9m
                                    }
                                },
                                LTVTierDetails=new List<LTVTierDetail>
                                {
                                    new LTVTierDetail
                                    {
                                        LTVStart=0.5m,
                                        LTVEnd=0.5m,
                                        TierRate=0.5m
                                    }
                                },
                                RateRange = new RateRange()
                                {
                                    MinimumRate=0.5m,
                                    MaximumRate=0.5m,
                                     AdditionalInformation = "Fixed promotional rate for selected customers"
                                }
                            }
                        },
                   // ✅ FIXED RATE
                    FixedRate = new FixedRateData
                    {
                        Description = "Fixed interest for initial period",
                        Rate = RandomRate(12.0m, 15.0m),
                        FixedRateEndDate = DateTime.UtcNow.AddYears(2),

                        CalculationFrequency = "Monthly",
                        ApplicationFrequency = "Monthly",
                        ProfitCalculationMethod = InterestCalculationMethodType.OutstandingBalance.ToString(),

                        AnnualPercentageRate = new AnnualPercentageRate
                        {
                            StartingFrom = "12.0",
                            UpTo = 15.0m,
                            AdditionalInformation = "Fixed period APR"
                        },

                        Tiers = new List<Tier>
                        {
                            new Tier
                            {
                                Name = "Fixed Tier",
                                Unit = TierUnitType.Balance,
                                ApplicationMethod = ApplicationMethodType.PerTier,

                                BalanceTierDetails = new List<BalanceTierDetail>
                                {
                                    new BalanceTierDetail
                                    {
                                        MinimumTierValue = new MoneyAmount { Amount = "10", Currency = "AED" },
                                        MaximumTierValue = new MoneyAmount { Amount = "500000", Currency = "AED" },
                                        TierRate = 13.5m
                                    }
                                },
                                LTVTierDetails=new List<LTVTierDetail>
                                {
                                    new LTVTierDetail
                                    {
                                        LTVStart=0.5m,
                                        LTVEnd=0.5m,
                                        TierRate=0.5m
                                    }
                                },
                                RateRange = new RateRange()
                                {
                                    MinimumRate=0.5m,
                                    MaximumRate=0.5m,
                                     AdditionalInformation = "Fixed promotional rate for selected customers"
                                }
                            }
                        },
                        FixedRateEnd= "2028-12-31",
                        IntroductoryPeriodOptions = new List<IntroductoryPeriodOptions>
                        {
                           new IntroductoryPeriodOptions
                           {
                                Period = "P2Y3M",
                                IndicativeRate = new IndicativeRate
                                {
                                    StartingFrom = 2.1m,
                                    UpTo = 3.05m
                                }
                           } 
                        }
                    },
                    // ✅ VARIABLE RATE
                    VariableRate = new VariableRateData
                    {
                        Description = "Variable interest after fixed period",
                        Rate = RandomRate(15.0m, 22.0m),
                        BenchMark = "EIBOR",
                        BenchMarkRate = RandomRate(10.0m, 15.0m),
                        Margin = RandomRate(2.0m, 4.0m),

                        RateReviewFrequency = "P6M",
                        RateReviewNextDate = DateTime.UtcNow.AddMonths(6),

                        CalculationFrequency = "Monthly",
                        ApplicationFrequency = "Monthly",
                        ProfitCalculationMethod = InterestCalculationMethodType.OutstandingBalance.ToString(),

                        AnnualPercentageRate = new AnnualPercentageRate
                        {
                            StartingFrom = "12.0",
                            UpTo = 22.0m,
                            AdditionalInformation = "Variable APR"
                        },

                        Tiers = new List<Tier>
                        {
                            new Tier
                            {
                                Name = "Variable Tier",
                                Unit = TierUnitType.Balance,
                                ApplicationMethod = ApplicationMethodType.PerTier,

                                BalanceTierDetails = new List<BalanceTierDetail>
                                {
                                    new BalanceTierDetail
                                    {
                                        MinimumTierValue = new MoneyAmount { Amount = "10", Currency = "AED" },
                                        MaximumTierValue = new MoneyAmount { Amount = "500000", Currency = "AED" },
                                        TierRate = 18.5m
                                    }
                                },
                                 LTVTierDetails=new List<LTVTierDetail>
                                {
                                    new LTVTierDetail
                                    {
                                        LTVStart=0.5m,
                                        LTVEnd=0.5m,
                                        TierRate=0.5m
                                    }
                                },
                                RateRange = new RateRange()
                                {
                                    MinimumRate=0.5m,
                                    MaximumRate=0.5m,
                                     AdditionalInformation = "Fixed promotional rate for selected customers"
                                }
                            }
                        },

                        VariableTerm = "P2Y3M"
                    },

                   // ✅ COMMON
                    Conditions = new List<Condition>
                    {
                        new Condition
                        {
                            Field = "CreditScore",
                            Operator = ">=",
                            Value = "700",
                            Description = "Better rates for high credit score"
                        }
                    },
                    Notes="string",
                    AdditionalInformation = new List<AdditionalInformationItem>
                    {
                        new AdditionalInformationItem
                        {
                            Type = AdditionalInformationType.Other,
                            Description = "Hybrid loan: fixed + variable"
                        }
                    }
        }
    }
                    };
                    productDetails.Tenor = new List<Tenor> { new Tenor { MinimumTenor = "P5Y", MaximumTenor = "P25Y", Condition = "Subject to approval" } };
                    productDetails.AssetBacked = new List<AssetBacked>
                    {
                            new AssetBacked
                            {
                                Type = AssetBackedType.Collateral,
                                AssetType = AssetType.Property,
                                Description = "string",

                                Valuation = new List<Valuation>
                                {
                                    new Valuation
                                    {
                                        Date = new DateTime(1970, 1, 1),
                                        Amount = new MoneyAmount
                                        {
                                            Amount = "100000", // replace with actual value
                                            Currency = "AED"
                                        }
                                    }
                                },

                                SupplementaryInformation = new Dictionary<string, object>(),

                                OwnershipTransfer = new OwnershipTransfer
                                {
                                    TransferOfOwnershipDate = new DateTime(1970, 1, 1),
                                    Type = OwnershipTransferType.Gift,
                                    Method = OwnershipTransferMethodType.EndOfLease,

                                    TokenPurchaseAmount = new MoneyAmount
                                    {
                                        Amount = "5000",
                                        Currency = "AED"
                                    },

                                    BuyoutSchedule = new BuyoutSchedule
                                    {
                                        Frequency = PaymentFrequencyType.Weekly,
                                        BuyoutAmount = new MoneyAmount
                                        {
                                            Amount = "1000",
                                            Currency = "AED"
                                        }
                                    },

                                    SaleAgreement = new SaleAgreement
                                    {
                                        Required = false,
                                        Execution =SaleAgreementExecutionType.AtLeaseCompletion,
                                        Price = new MoneyAmount
                                        {
                                            Amount = "120000",
                                            Currency = "AED"
                                        }
                                    },

                                    TransferConditions = new List<TransferConditionType>
                                    {
                                        TransferConditionType.AllLeasePaymentsCompleted
                                    }
                                }
                            }
                    };
                    break;
                
                default:
                    logger.Warn($"Unknown category: {category}");
                    return response;
            }

            product.Product = productDetails;
            lFIData.Products!.Add(product);
            lfiDataList.Add(lFIData);
            response.Data = lfiDataList;
            response.Meta = new LFIMeta { TotalPages = 1, TotalRecords = 1 };
        }
        catch (Exception ex)
        {
            logger.Error($"Error in ResponseProductDetails(): {ex}");
            throw;
        }

        return response;
    }

    private StringContent GetStringContent(string jsonContent, string correlationId)
    {
        try
        {
            return new StringContent(jsonContent, Encoding.UTF8, "application/json");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error during encryption/decryption", ex);
        }
    }
}

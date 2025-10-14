using ConsentModel.Common;
using OF.ProductData.CentralBankConn.API.IServices;
using OF.ProductData.CentralBankConn.API.Repositories;
using OF.ProductData.Common.Custom;
using OF.ProductData.Common.Helpers;
using OF.ProductData.Common.NLog;
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
    public ProductDataService(HttpClient httpClient, IOptions<CoreBankApis> coreBankApis, IMasterRepository masterRepository,IOptions<ApiHeaderParams> apiHeaderParams, Custom custom, ProductLogger logger)
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
               
                var coreBankProductResponse = JsonConvert.DeserializeObject<CbsProductResponse>(apiResponseBody) ?? throw new InvalidOperationException("Deserialized coreBankProductResponse is null.");
                apiResult = ApiResultFactory.Success(data: coreBankProductResponse!, apiResponseBody, "200");
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

    //public CbProductResponse GetCentralBankProductByIdResponse(CbsProductResponse cbProductResponse, Logger logger)
    //{
    //    var random = new Random();

    //    try
    //    {
    //        var response = new CbProductResponse
    //        {
    //            Data = new List<LFIData>
    //        {
    //            new LFIData
    //            {
    //                LFIId = Guid.NewGuid().ToString(),
    //                LFIBrandId = "Brand_" + random.Next(1000, 9999),
    //                Products = new List<ProductWrapper>
    //                {
    //                    new ProductWrapper
    //                    {
    //                        ProductId = "Prod_" + random.Next(10000, 99999),
    //                        ProductName = "Savings Account",
    //                        ProductCategory = "SavingsAccount",
    //                        Description = "Dummy savings account product",
    //                        EffectiveFromDateTime = DateTime.UtcNow.AddYears(-1),
    //                        EffectiveToDateTime = DateTime.UtcNow.AddYears(1),
    //                        LastUpdatedDateTime = DateTime.UtcNow,
    //                        IsShariaCompliant = false,
    //                        ShariaInformation = "Not Applicable",
    //                        IsSalaryTransferRequired = false,
    //                        Links = new Links
    //                        {
    //                            ApplicationUri = "http://example.com/apply",
    //                            ApplicationPhoneNumber = "+971500000000",
    //                            ApplicationEmail = "support@example.com",
    //                            ApplicationDescription = "Application info",
    //                            KfsUri = "http://example.com/kfs",
    //                            OverviewUri = "http://example.com/overview",
    //                            TermsUri = "http://example.com/terms",
    //                            FeesAndPricingUri = "http://example.com/fees",
    //                            ScheduleOfChargesUri = "http://example.com/schedule",
    //                            EligibilityUri = "http://example.com/eligibility",
    //                            CardImageUri = "http://example.com/card.png"
    //                        },
    //                        Eligibility = new EligibilityData
    //                        {
    //                            ResidenceStatus = new List<TypeDescription>
    //                            {
    //                                new TypeDescription { Type = "UaeResident", Description = "A person who is a resident of the UAE." }
    //                            },
    //                            EmploymentStatus = new List<TypeDescription>
    //                            {
    //                                new TypeDescription { Type = "Salaried", Description = "Salaried employee" }
    //                            },
    //                            CustomerType = new List<TypeDescription>
    //                            {
    //                                new TypeDescription { Type = "Retail", Description = "Retail customer" }
    //                            },
    //                            AccountOwnership = new List<TypeDescription>
    //                            {
    //                                new TypeDescription { Type = "Individual", Description = "Individual ownership" }
    //                            },
    //                            Age = new List<AgeEligibility>
    //                            {
    //                                new AgeEligibility { Type = "MinimumAge", Description = "Minimum 18 years", Value = 18 }
    //                            },
    //                            AdditionalEligibility = new List<TypeDescription>
    //                            {
    //                                new TypeDescription { Type = "Student", Description = "Student eligibility" }
    //                            }
    //                        },
    //                        Channels = new List<Channel>
    //                        {
    //                            new Channel { Type = "Phone", Description = "Apply via phone" }
    //                        },
    //                        Product = new ProductDetails
    //                        {
    //                            SavingsAccount = new SavingsAccountData
    //                            {
    //                                Type = "Savings",
    //                                Description = "Dummy savings account",
    //                                MinimumBalance = new Amount { AmountValue = "1000", Currency = "AED" },
    //                                AnnualReturn = 1.5,
    //                                Documentation = new List<Document>
    //                                {
    //                                    new Document { Type = "ApplicationForm", Description = "Fill application form" }
    //                                },
    //                                Features = new List<Feature>
    //                                {
    //                                    new Feature { Type = "IslamicBanking", Description = "Sharia compliant feature" }
    //                                },
    //                                Fees = new List<Fee>
    //                                {
    //                                    new Fee
    //                                    {
    //                                        Type = "MonthlyFees",
    //                                        Period = "Monthly",
    //                                        Name = "Account Maintenance",
    //                                        Description = "Monthly account maintenance fee",
    //                                        Unit = "Amount",
    //                                        Amount = new Amount { AmountValue = "10", Currency = "AED" },
    //                                        Percentage = 0,
    //                                        UnitValue = 10,
    //                                        MaximumUnitValue = 10
    //                                    }
    //                                },
    //                                Limits = new List<Limit>
    //                                {
    //                                    new Limit { Type = "MinimumOpeningBalance", Description = "Minimum opening balance", Value = 1000 }
    //                                },
    //                                Benefits = new List<Benefit>
    //                                {
    //                                    new Benefit { Type = "Cashback", Name = "Monthly Cashback", Description = "Cashback benefit", Value = 50 }
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        };

    //        return response;
    //    }
    //    catch (Exception ex)
    //    {
    //        logger.Error(ex, "Error occurred in GetCentralBankProductByIdResponse()");
    //        throw;
    //    }
    //}

    public CbProductResponse GetCentralBankProductByIdResponse(CbsProductResponse cbProductResponse, Logger logger)
    {
        var random = new Random();

        try
        {
            var response = new CbProductResponse
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
                            ProductCategory = "MultiProduct",
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
                                ResidenceStatus = new List<TypeDescription> { new TypeDescription { Type = "UaeResident", Description = "A person who is a resident of the UAE." } },
                                EmploymentStatus = new List<TypeDescription> { new TypeDescription { Type = "Salaried", Description = "Salaried employee" } },
                                CustomerType = new List<TypeDescription> { new TypeDescription { Type = "Retail", Description = "Retail customer" } },
                                AccountOwnership = new List<TypeDescription> { new TypeDescription { Type = "Individual", Description = "Individual ownership" } },
                                Age = new List<AgeEligibility> { new AgeEligibility { Type = "MinimumAge", Description = "Minimum 18 years", Value = 18 } },
                                AdditionalEligibility = new List<TypeDescription> { new TypeDescription { Type = "Student", Description = "Student eligibility" } }
                            },
                            Channels = new List<Channel> { new Channel { Type = "Phone", Description = "Apply via phone" } },
                            Product = new ProductDetails
                            {
                                CurrentAccount = new CurrentAccountData
                                {
                                    Type = "Basic",
                                    Description = "Dummy current account",
                                    IsOverdraftAvailable = false,
                                    Documentation = new List<Document> { new Document { Type = "ApplicationForm", Description = "Fill application form" } },
                                    Features = new List<Feature> { new Feature { Type = "IslamicBanking", Description = "Sharia compliant feature" } },
                                    Fees = new List<Fee> { new Fee { Type = "MonthlyFees", Period = "Daily", Name = "Account Maintenance", Description = "Monthly account maintenance fee", Unit = "Amount", Amount = new Amount { AmountValue = "10", Currency = "AED" }, Percentage = 0.5, UnitValue = 0.5, MaximumUnitValue = 0.5 } },
                                    Limits = new List<Limit> { new Limit { Type = "MinimumDeposit", Description = "Minimum deposit", Value = 0.5 } },
                                    Benefits = new List<Benefit> { new Benefit { Type = "Cashback", Name = "Monthly Cashback", Description = "Cashback benefit", Value = 0.5 } }
                                },
                                SavingsAccount = new SavingsAccountData
                                {
                                    Type = "Savings",
                                    Description = "Dummy savings account",
                                    MinimumBalance = new Amount { AmountValue = "1000", Currency = "AED" },
                                    AnnualReturn = 1.5,
                                    Documentation = new List<Document> { new Document { Type = "ApplicationForm", Description = "Fill application form" } },
                                    Features = new List<Feature> { new Feature { Type = "IslamicBanking", Description = "Sharia compliant feature" } },
                                    Fees = new List<Fee> { new Fee { Type = "MonthlyFees", Period = "Daily", Name = "Account Maintenance", Description = "Monthly account maintenance fee", Unit = "Amount", Amount = new Amount { AmountValue = "10", Currency = "AED" }, Percentage = 0.5, UnitValue = 0.5, MaximumUnitValue = 0.5 } },
                                    Limits = new List<Limit> { new Limit { Type = "MinimumOpeningBalance", Description = "Minimum opening balance", Value = 1000 } },
                                    Benefits = new List<Benefit> { new Benefit { Type = "Cashback", Name = "Monthly Cashback", Description = "Cashback benefit", Value = 50 } }
                                },
                                CreditCard = new CreditCardData
                                {
                                    Type = "Visa",
                                    Description = "Dummy credit card",
                                    Rate = 0.5m,
                                    Documentation = new List<Document> { new Document { Type = "ApplicationForm", Description = "Fill application form" } },
                                    Features = new List<Feature> { new Feature { Type = "IslamicBanking", Description = "Sharia compliant feature" } },
                                    Fees = new List<Fee> { new Fee { Type = "AnnualFee", Period = "Daily", Name = "Annual Fee", Description = "Annual fee", Unit = "Amount", Amount = new Amount { AmountValue = "100", Currency = "AED" }, Percentage = 0.5, UnitValue = 0.5, MaximumUnitValue = 0.5 } },
                                    Limits = new List<Limit> { new Limit { Type = "MinimumCreditLimit", Description = "Minimum credit limit", Value = 5000 } },
                                    Benefits = new List<Benefit> { new Benefit { Type = "Cashback", Name = "Cashback", Description = "Cashback benefit", Value = 50 } }
                                },
                                PersonalLoan = new PersonalLoanData
                                {
                                    Type = "PersonalFinance",
                                    Description = "Dummy personal loan",
                                    MinimumLoanAmount = new Amount { AmountValue = "5000", Currency = "AED" },
                                    MaximumLoanAmount = new Amount { AmountValue = "50000", Currency = "AED" },
                                    Tenure = new LoanTenure { MinimumLoanTenure = 1, MaximumLoanTenure = 5 },
                                    CalculationMethod = "FlatRate",
                                    Rate = new RateDetails
                                    {
                                        Type = "Fixed",
                                        Description = "Fixed rate",
                                        ReviewFrequency = "Annually",
                                        IndicativeRate = new APR { From = 0.5m, To = 1.0m },
                                        ProfitRate = new APR { From = 0.5m, To = 1.0m }
                                    },
                                    AnnualPercentageRateRange = new APR { From = 0.5m, To = 1.5m },
                                    FixedRatePeriod = "1 year",
                                    DebtBurdenRatio = "40%",
                                    Documentation = new List<Document> { new Document { Type = "ApplicationForm", Description = "Fill application form" } },
                                    Features = new List<Feature> { new Feature { Type = "IslamicBanking", Description = "Sharia compliant feature" } },
                                    Limits = new List<Limit> { new Limit { Type = "MinimumRequiredCreditScore", Description = "Minimum credit score", Value = 600 } },
                                    Fees = new List<Fee> { new Fee { Type = "Processing", Period = "Daily", Name = "Processing Fee", Description = "Processing fee", Unit = "Amount", Amount = new Amount { AmountValue = "100", Currency = "AED" }, Percentage = 0.5, UnitValue = 0.5, MaximumUnitValue = 0.5 } },
                                    Benefits = new List<Benefit> { new Benefit { Type = "Other", Name = "Other Benefit", Description = "Other benefit", Value = 0.5 } },
                                    AdditionalInformation = new List<AdditionalInformation> { new AdditionalInformation { Type = "Other", Description = "Additional info" } }
                                },
                                Mortgage = new MortgageData
                                {
                                    Type = "FixedRate",
                                    Description = "Dummy mortgage",
                                    CalculationMethod = "FlatRate",
                                    Structure = "Standard",
                                    Rate = new RateDetails { Type = "Fixed", Description = "Fixed rate", ReviewFrequency = "Annually", IndicativeRate = new APR { From = 0.5m, To = 1.0m }, ProfitRate = new APR { From = 0.5m, To = 1.0m } },
                                    IndicativeAPR = new APR { From = 0.5m, To = 1.0m },
                                    FixedRatePeriod = "1 year",
                                    MinimumLoanAmount = new Amount { AmountValue = "100000", Currency = "AED" },
                                    MaximumLoanAmount = new Amount { AmountValue = "1000000", Currency = "AED" },
                                    MaximumLTV = 0.8,
                                    DownPayment = new Amount { AmountValue = "20000", Currency = "AED" },
                                    Tenure = new LoanTenure { MinimumLoanTenure = 5, MaximumLoanTenure = 25 },
                                    Documentation = new List<Document> { new Document { Type = "ApplicationForm", Description = "Fill application form" } },
                                    Features = new List<Feature> { new Feature { Type = "IslamicBanking", Description = "Sharia compliant feature" } },
                                    Fees = new List<Fee> { new Fee { Type = "Processing", Period = "Daily", Name = "Processing Fee", Description = "Processing fee", Unit = "Amount", Amount = new Amount { AmountValue = "100", Currency = "AED" }, Percentage = 0.5, UnitValue = 0.5, MaximumUnitValue = 0.5 } },
                                    Limits = new List<Limit> { new Limit { Type = "MinimumRequiredCreditScore", Description = "Minimum credit score", Value = 600, Percentage = 0.5 } },
                                    Benefits = new List<Benefit> { new Benefit { Type = "Other", Name = "Other Benefit", Description = "Other benefit", Value = 0.5 } }
                                },
                                ProfitSharingRate = new ProfitSharingRateData
                                {
                                    Name = "Dummy Profit Sharing",
                                    Description = "Profit sharing account",
                                    MinimumDepositAmount = new Amount { AmountValue = "1000", Currency = "AED" },
                                    AnnualReturn = 1.0m,
                                    AnnualReturnOptions = new List<NameDescription> { new NameDescription { Name = "Option1", Description = "Return Option 1" } },
                                    InvestmentPeriod = new NameDescription { Name = "1 Year", Description = "Investment period" },
                                    AdditionalInformation = new List<AdditionalInformation> { new AdditionalInformation { Type = "Info", Description = "Additional info" } }
                                },
                                FinanceProfitRate = new FinanceProfitRateData
                                {
                                    Name = "Dummy Finance Profit",
                                    Description = "Finance profit rate",
                                    CalculationMethod = "FlatRate",
                                    Rate = 1.0m,
                                    Frequency = "Monthly",
                                    Tiers = new List<Tier> { new Tier { Type = "Tier1", Unit = "AED", MinimumTierValue = new Amount { AmountValue = "1000", Currency = "AED" }, MaximumTierValue = new Amount { AmountValue = "5000", Currency = "AED" }, MinimumTierRate = 0.5m, MaximumTierRate = 1.0m, Condition = "Basic Condition" } },
                                    AdditionalInformation = new List<AdditionalInformation> { new AdditionalInformation { Type = "Info", Description = "Additional info" } }
                                }
                            }
                        }
                    }
                }
            }
            };

            return response;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error occurred in GetCentralBankProductByIdResponse()");
            throw;
        }
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

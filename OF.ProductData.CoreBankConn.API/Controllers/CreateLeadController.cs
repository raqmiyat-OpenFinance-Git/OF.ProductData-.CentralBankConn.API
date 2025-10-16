using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OF.ProductData.Common.Helpers;
using OF.ProductData.Common.NLog;
using OF.ProductData.CoreBankConn.API.IServices;
using OF.ProductData.CoreBankConn.API.Repositories;
using OF.ProductData.Model.CentralBank.Products;
using OF.ProductData.Model.Common;
using OF.ProductData.Model.CoreBank;
using OF.ProductData.Model.CoreBank.Products;
using System.Net;

namespace OF.ProductData.CoreBankConn.API.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CreateLeadController : ControllerBase
{
    private readonly ICreateLeadService _Service;
    private readonly ICoreBankRepository _repository;
    private readonly ProductLogger _Logger;
    private readonly WarmUpLogger _warmUpLogger;
    private readonly IOptions<ServiceParams> _serviceParams;

    public CreateLeadController(ICreateLeadService EnquiryService, ICoreBankRepository repository, IOptions<ServiceParams> serviceParams, ProductLogger enquiryLogger, WarmUpLogger warmUpLogger)
    {
        _Service = EnquiryService;
        _repository = repository;
        _serviceParams = serviceParams;
        _Logger = enquiryLogger;
        _warmUpLogger = warmUpLogger;
    }

    [HttpGet]
    [Route("warmup")]
    public async Task<IActionResult> WarmUp()
    {
        try
        {
            var response = await _Service.WarmUp(_warmUpLogger.Log);

            if (response == HttpStatusCode.OK)
            {
                return Ok("Warm-up complete.");
            }

            return StatusCode((int)response, "Warm-up failed.");
        }
        catch (Exception ex)
        {
            _warmUpLogger.Error(ex, $"An error occurred during the warm-up process. Exception details: {ex}");
            return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred during warm-up.");
        }
    }

    [HttpPost]
    [Route("CreateLead")]
    public async Task<IActionResult> GetProductData(CbsCreateLeadRequest coreBankProductRequest)
    {
        var coreBankproductResponse = new ApiResult<CbsCreateLeadResponse>
        {
            Success = false,
            Code = "400",
            Message = string.Empty,
            Data = null
        };
        try
        {
            ApiResult<CbsCreateLeadResponse>? result = null;
            if (_serviceParams.Value.Online)
            {
                if (!ModelState.IsValid)
                {
                    coreBankproductResponse.Message = "Invalid request model.";
                    return BadRequest(coreBankproductResponse);
                }

                if (coreBankProductRequest != null)
                {
                    await PrepareEnquiryRequest(coreBankProductRequest);

                    result = await _Service.PostOnlineEnquiryAsync(coreBankProductRequest!, _Logger.Log);

                    if (!result.Success)
                    {
                        if (string.IsNullOrEmpty(result.Code))
                        {
                            return StatusCode(502, result); // or 504 depending on scenario
                        }

                        if (int.TryParse(result.Code, out int statusCode))
                        {
                            return StatusCode(statusCode, result);
                        }

                        return StatusCode(500, result); // General failure
                    }
                }


                return Ok(ApiResultFactory.Success(result!.Data!, "Product retrieved successfully"));
            }
            else
            {
                var coreBankproductdataResponse = new CbsCreateLeadResponse
                {
                    fullName = "Anbarasan Krishnan",
                    givenName = "Anbarasan",
                    familyName = "Krishnan",
                    birthDate = "1990-05-10",
                    emiratesId = "784-1990-1234567-1",
                    nationality = "Indian",
                    mobileNumber = "+971501234567",
                    email = "anbarasan@example.com",
                    maritalStatus = "Married",
                    employerName = "ABC Tech Solutions",
                    profession = "Software Engineer"
                };
                return Ok(ApiResultFactory.Success(coreBankproductdataResponse, "Product retrieved successfully"));
            }
        }
        catch (Exception ex)
        {
            _Logger.Error(ex);
            return BadRequest(ApiResultFactory.Failure<CbsCreateLeadResponse>("An unexpected error occurred."));
        }
    }

    private async Task PrepareEnquiryRequest(CbsCreateLeadRequest coreBankProductRequest)
    {
        try
        {
       

            coreBankProductRequest.ExternalRefNbr = await _repository
                .GetNextSequenceNoAsync("PRODUCTENQ", coreBankProductRequest!.ExternalRefNbr, _Logger.Log);

        }
        catch (Exception ex)
        {
            _Logger.Error(ex, $"An error occurred while preparing the enquiry request. Exception details: {ex}");
        }
    }



}

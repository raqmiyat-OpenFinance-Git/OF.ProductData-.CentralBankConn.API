using OF.ProductData.CentralBankConn.API.Repositories;
using OF.ProductData.Model.CentralBank.Products;
using OF.ProductData.Model.Common;

namespace OF.ProductData.CentralBankConn.API.Validators;


public class ProductDataRequestValidator : AbstractValidator<CbProductDataRequest>
{
    private readonly IMasterRepository _masterRepository;
    private readonly Logger _logger;

    public ProductDataRequestValidator(IMasterRepository masterRepository, Logger logger)
    {
        _masterRepository = masterRepository;
        _logger = logger;

        RuleFor(model => model.O3ProviderId)
             .Cascade(CascadeMode.Stop)
             .NotNull().WithMessage("o3-provider-id cannot be blank")
             .NotEmpty().WithMessage("o3-provider-id cannot be blank")
             .MaximumLength(100).WithMessage("o3-provider-id length should be maximum 100 characters.")
             .Matches("^[a-zA-Z0-9_-]*$").WithMessage("o3-provider-id must not contain special characters.")
             .MustAsync(ValidateDuplicateTransactionIdAsync)
             .WithMessage("Duplicate Transaction ID found.");

        //o3-aspsp-id
        RuleFor(model => model.O3AspspId)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("o3-aspsp-id cannot be blank")
            .NotEmpty().WithMessage("o3-aspsp-id cannot be blank")
            .MaximumLength(100).WithMessage("o3-aspsp-id length should be maximum 100 characters.")
            .Matches("^[a-zA-Z0-9_-]*$").WithMessage("o3-aspsp-id must not contain special characters.");

        //o3-caller-org-id
        RuleFor(model => model.O3CallerOrgId)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("o3-caller-org-id cannot be blank")
            .NotEmpty().WithMessage("o3-caller-org-id cannot be blank")
            .MaximumLength(100).WithMessage("o3-caller-org-id length should be maximum 100 characters.")
            .Matches("^[a-zA-Z0-9_-]*$").WithMessage("o3-caller-org-id must not contain special characters.");

        //o3-caller-client-id
        RuleFor(model => model.O3CallerClientId)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("o3-caller-client-id cannot be blank")
            .NotEmpty().WithMessage("o3-caller-client-id cannot be blank")
            .MaximumLength(100).WithMessage("o3-caller-client-id length should be maximum 100 characters.")
            .Matches("^[a-zA-Z0-9_-]*$").WithMessage("o3-caller-client-id must not contain special characters.");

        //o3-caller-software-statement-id
        RuleFor(model => model.O3CallerSoftwareStatementId)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("o3-caller-software-statement-id cannot be blank")
            .NotEmpty().WithMessage("o3-caller-software-statement-id cannot be blank")
            .MaximumLength(100).WithMessage("o3-caller-software-statement-id length should be maximum 100 characters.")
            .Matches("^[a-zA-Z0-9_-]*$").WithMessage("o3-caller-software-statement-id must not contain special characters.");

        //o3-api-uri
        RuleFor(model => model.O3ApiUri)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("o3-api-uri cannot be blank")
            .NotEmpty().WithMessage("o3-api-uri cannot be blank")
            .MaximumLength(100).WithMessage("o3-api-uri length should be maximum 100 characters.")
            .Matches("^[a-zA-Z0-9_-]*$").WithMessage("o3-api-uri must not contain special characters.");

        //o3-api-operation
        RuleFor(model => model.O3ApiOperation)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("o3-api-operation cannot be blank")
            .NotEmpty().WithMessage("o3-api-operation cannot be blank")
            .MaximumLength(100).WithMessage("o3-api-operation length should be maximum 100 characters.")
            .Matches("^[a-zA-Z0-9_-]*$").WithMessage("o3-api-operation must not contain special characters.");
        //o3-caller-interaction-id
        RuleFor(model => model.O3CallerInteractionId)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("o3-caller-interaction-id cannot be blank")
            .NotEmpty().WithMessage("o3-caller-interaction-id cannot be blank")
            .MaximumLength(100).WithMessage("o3-caller-interaction-id length should be maximum 100 characters.")
            .Matches("^[a-zA-Z0-9_-]*$").WithMessage("o3-caller-interaction-id must not contain special characters.");

        //o3-ozone-interaction-id
        RuleFor(model => model.O3OzoneInteractionId)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("o3-ozone-interaction-id cannot be blank")
            .NotEmpty().WithMessage("o3-ozone-interaction-id cannot be blank")
            .MaximumLength(100).WithMessage("o3-ozone-interaction-id length should be maximum 100 characters.")
            .Matches("^[a-zA-Z0-9_-]*$").WithMessage("o3-ozone-interaction-id must not contain special characters.");

        //CustomerIpAddress
        RuleFor(model => model.CustomerIpAddress)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("o3-aspsp-id cannot be blank")
            .NotEmpty().WithMessage("o3-aspsp-id cannot be blank")
            .MaximumLength(100).WithMessage("o3-aspsp-id length should be maximum 100 characters.")
            .Matches("^[a-zA-Z0-9_-]*$").WithMessage("o3-aspsp-id must not contain special characters.");


        //SortField
        RuleFor(model => model.SortField)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("SortField cannot be blank")
            .NotEmpty().WithMessage("SortField cannot be blank")
            .MaximumLength(100).WithMessage("SortField length should be maximum 100 characters.")
            .Matches("^[a-zA-Z0-9_-]*$").WithMessage("SortField must not contain special characters.");

        //SortOrder
        RuleFor(model => model.SortOrder)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("SortOrder cannot be blank")
            .NotEmpty().WithMessage("SortOrder cannot be blank")
            .MaximumLength(100).WithMessage("SortOrder length should be maximum 100 characters.")
            .Matches("^[a-zA-Z0-9_-]*$").WithMessage("SortOrder must not contain special characters.");

        //LastUpdatedDateTime
        RuleFor(model => model.LastUpdatedDateTime)
      .Cascade(CascadeMode.Stop)
      .NotNull().WithMessage("LastUpdatedDateTime cannot be null.")
      .LessThanOrEqualTo(DateTime.UtcNow)
      .WithMessage("LastUpdatedDateTime cannot be in the future.");



        //accountId 
        RuleFor(model => model.ProductCategory)
           .Cascade(CascadeMode.Stop)
           .NotNull().WithMessage("ProductCategory cannot be blank")
           .NotEmpty().WithMessage("ProductCategory cannot be blank")
           .MaximumLength(100).WithMessage("accountId length should be maximum 100 characters.")
           .Matches("^[a-zA-Z0-9_-]*$").WithMessage("accountId must not contain special characters.");

        //Page
        RuleFor(model => model.PageNumber)
    .GreaterThan(0).WithMessage("Page must be greater than 0.");

        //pageSize

        RuleFor(model => model.PageSize)
            .InclusiveBetween(1, 1000).WithMessage("PageSize must be between 1 and 1000.");

    }

    private async Task<bool> ValidateDuplicateTransactionIdAsync(CbProductDataRequest model, string o3ProviderId, CancellationToken cancellationToken)
    {
        try
        {
            _logger.Info("ValidateDuplicateTransactionIdAsync is invoked.");
            _logger.Info($"Validating transaction ID: {o3ProviderId}");

            if (string.IsNullOrEmpty(o3ProviderId))
            {
                return true; // Already handled by NotEmpty, skip this check
            }

            bool isDuplicate = await _masterRepository.IsDuplicateTransactionIdAsync(
                MessageTypeMappings.ProductEnquiry, o3ProviderId,
                _logger);

            _logger.Info("ValidateDuplicateTransactionIdAsync is done.");
            return !isDuplicate;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred in ValidateDuplicateTransactionIdAsync()");
            return false; // Fail validation if exception occurs
        }
    }
}

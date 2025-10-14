using OF.ProductData.CentralBankConn.API.Repositories;
using OF.ProductData.Model.CentralBank.Products;
using OF.ProductData.Model.Common;

namespace OF.ProductData.CentralBankConn.API.Validators;


public class ProductDataRequestValidator : AbstractValidator<CbProductRequest>
{
    private readonly IMasterRepository _masterRepository;
    private readonly Logger _logger;

    public ProductDataRequestValidator(IMasterRepository masterRepository, Logger logger)
    {
        _masterRepository = masterRepository;
        _logger = logger;

        //Authorization
        RuleFor(model => model.Authorization)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Authorization cannot be blank")
            .NotEmpty().WithMessage("Authorization cannot be blank")
            .MaximumLength(100).WithMessage("Authorization length should be maximum 100 characters.")
            .Matches("^[a-zA-Z0-9_-]*$").WithMessage("Authorization must not contain special characters.")
            .MustAsync(ValidateDuplicateTransactionIdAsync);
         

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

    private async Task<bool> ValidateDuplicateTransactionIdAsync(CbProductRequest model, string o3ProviderId, CancellationToken cancellationToken)
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

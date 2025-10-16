using OF.ProductData.CentralBankConn.API.Repositories;
using OF.ServiceInitiation.Model.CentralBank.Payments.PostHeader;

namespace OF.ProductData.CentralBankConn.API.Validators;


public class CreateLeadRequestValidator : AbstractValidator<CbPostCreateLeadHeader>
{
    private readonly IMasterRepository _masterRepository;
    private readonly Logger _logger;

    public CreateLeadRequestValidator(IMasterRepository masterRepository, Logger logger)
    {
        _masterRepository = masterRepository;
        _logger = logger;

        RuleFor(model => model.O3ProviderId)
             .Cascade(CascadeMode.Stop)
             .NotNull().WithMessage("o3-provider-id cannot be blank")
             .NotEmpty().WithMessage("o3-provider-id cannot be blank")
             .MaximumLength(100).WithMessage("o3-provider-id length should be maximum 100 characters.")
             .Matches("^[a-zA-Z0-9_-]*$").WithMessage("o3-provider-id must not contain special characters.");
            

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
        RuleFor(model => model.XFapiCustomerIpAddress)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("o3-aspsp-id cannot be blank")
            .NotEmpty().WithMessage("o3-aspsp-id cannot be blank")
            .MaximumLength(100).WithMessage("o3-aspsp-id length should be maximum 100 characters.")
            .Matches("^[a-zA-Z0-9_-]*$").WithMessage("o3-aspsp-id must not contain special characters.");

    }  
}

using FluentValidation;
using Webhooks.API.Model;

namespace Webhooks.API.Validators;

public class WebhookSubscriptionRequestValidator : AbstractValidator<WebhookSubscriptionRequest>
{
    public WebhookSubscriptionRequestValidator()
    {
        RuleFor(x => x.GrantUrl)
            .NotEmpty().WithMessage("GrantUrl is required")
            .Must(u => Uri.IsWellFormedUriString(u, UriKind.Absolute)).WithMessage("GrantUrl is not valid");

        RuleFor(x => x.Url)
            .NotEmpty().WithMessage("Url is required")
            .Must(u => Uri.IsWellFormedUriString(u, UriKind.Absolute)).WithMessage("Url is not valid");

        RuleFor(x => x.Event)
            .NotEmpty().WithMessage("Event is required")
            .Must(e => Enum.TryParse(typeof(WebhookType), e, ignoreCase: true, out _))
            .WithMessage((request, evt) => $"{evt} is invalid event name");
    }
}

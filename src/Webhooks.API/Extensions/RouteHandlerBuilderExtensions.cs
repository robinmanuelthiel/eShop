namespace Webhooks.API.Extensions;

using FluentValidation;
using Webhooks.API.Model;

public static class RouteHandlerBuilderExtensions
{
    public static RouteHandlerBuilder ValidateWebhookSubscriptionRequest(this RouteHandlerBuilder routeHandlerBuilder)
    {
        return routeHandlerBuilder.AddEndpointFilter(async (context, next) =>
        {
            var webhookSubscriptionRequest = context.Arguments.OfType<WebhookSubscriptionRequest>().SingleOrDefault();

            if (webhookSubscriptionRequest == null)
            {
                return TypedResults.BadRequest("No WebhookSubscriptionRequest found.");
            }

            var validator = context.HttpContext.RequestServices.GetService(typeof(IValidator<WebhookSubscriptionRequest>)) as IValidator<WebhookSubscriptionRequest>;
            if (validator == null)
            {
                // No validator registered - continue without validation
                return await next(context);
            }

            var validationResult = await validator.ValidateAsync(webhookSubscriptionRequest);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => string.IsNullOrWhiteSpace(e.PropertyName) ? string.Empty : e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray());

                return TypedResults.ValidationProblem(errors);
            }

            return await next(context);
        });
    }

    private static Dictionary<string, string[]> ToErrors(this IEnumerable<ValidationResult> validationResults)
    {
        Dictionary<string, string[]> errors = [];

        foreach (var validationResult in validationResults)
        {
            var propertyNames = validationResult.MemberNames.Any() ? validationResult.MemberNames : [string.Empty];

            foreach (string propertyName in propertyNames)
            {
                if (errors.TryGetValue(propertyName, out var value))
                {
                    errors[propertyName] = [..value, validationResult.ErrorMessage];
                }
                else
                {
                    errors.Add(propertyName, [validationResult.ErrorMessage]);  
                }
            }
        }
        return errors;
    }
}

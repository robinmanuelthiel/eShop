using System.Text;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace eShop.ServiceDefaults;

internal static class OpenApiOptionsExtensions
{
    public static SwaggerGenOptions ApplyApiVersionInfo(this SwaggerGenOptions options, string title, string description)
    {
        options.DocumentFilter<ApiVersionDocumentFilter>(title, description);
        return options;
    }

    private static string BuildDescription(ApiVersionDescription api, string description)
    {
        var text = new StringBuilder(description);

        if (api.IsDeprecated)
        {
            if (text.Length > 0)
            {
                if (text[^1] != '.')
                {
                    text.Append('.');
                }

                text.Append(' ');
            }

            text.Append("This API version has been deprecated.");
        }

        if (api.SunsetPolicy is { } policy)
        {
            if (policy.Date is { } when)
            {
                if (text.Length > 0)
                {
                    text.Append(' ');
                }

                text.Append("The API will be sunset on ")
                    .Append(when.Date.ToShortDateString())
                    .Append('.');
            }

            if (policy.HasLinks)
            {
                text.AppendLine();

                var rendered = false;

                foreach (var link in policy.Links.Where(l => l.Type == "text/html"))
                {
                    if (!rendered)
                    {
                        text.Append("<h4>Links</h4><ul>");
                        rendered = true;
                    }

                    text.Append("<li><a href=\"");
                    text.Append(link.LinkTarget.OriginalString);
                    text.Append("\">");
                    text.Append(
                        StringSegment.IsNullOrEmpty(link.Title)
                        ? link.LinkTarget.OriginalString
                        : link.Title.ToString());
                    text.Append("</a></li>");
                }

                if (rendered)
                {
                    text.Append("</ul>");
                }
            }
        }

        return text.ToString();
    }

    public static SwaggerGenOptions ApplySecuritySchemeDefinitions(this SwaggerGenOptions options)
    {
        options.DocumentFilter<SecuritySchemeDocumentFilter>();
        return options;
    }

    public static SwaggerGenOptions ApplyAuthorizationChecks(this SwaggerGenOptions options, string[] scopes)
    {
        options.OperationFilter<AuthorizationOperationFilter>(scopes);
        return options;
    }

    public static SwaggerGenOptions ApplyOperationDeprecatedStatus(this SwaggerGenOptions options)
    {
        options.OperationFilter<DeprecatedOperationFilter>();
        return options;
    }

    private static IOpenApiAny? CreateOpenApiAnyFromObject(object value)
    {
        return value switch
        {
            bool b => new OpenApiBoolean(b),
            int i => new OpenApiInteger(i),
            double d => new OpenApiDouble(d),
            string s => new OpenApiString(s),
            _ => null
        };
    }

    private class ApiVersionDocumentFilter : IDocumentFilter
    {
        private readonly string _title;
        private readonly string _description;
        private readonly IApiVersionDescriptionProvider _provider;

        public ApiVersionDocumentFilter(string title, string description, IApiVersionDescriptionProvider provider)
        {
            _title = title;
            _description = description;
            _provider = provider;
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var apiDescription = _provider.ApiVersionDescriptions
                .SingleOrDefault(description => description.GroupName == context.DocumentName);
            if (apiDescription is null)
            {
                return;
            }
            swaggerDoc.Info.Version = apiDescription.ApiVersion.ToString();
            swaggerDoc.Info.Title = _title;
            swaggerDoc.Info.Description = BuildDescription(apiDescription, _description);
        }
    }

    private class SecuritySchemeDocumentFilter : IDocumentFilter
    {
        private readonly IConfiguration _configuration;

        public SecuritySchemeDocumentFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var identitySection = _configuration.GetSection("Identity");
            if (!identitySection.Exists())
            {
                return;
            }

            var identityUrlExternal = identitySection.GetRequiredValue("Url");
            var scopes = identitySection.GetRequiredSection("Scopes").GetChildren().ToDictionary(p => p.Key, p => p.Value);
            var securityScheme = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows()
                {
                    // TODO: Change this to use Authorization Code flow with PKCE
                    Implicit = new OpenApiOAuthFlow()
                    {
                        AuthorizationUrl = new Uri($"{identityUrlExternal}/connect/authorize"),
                        TokenUrl = new Uri($"{identityUrlExternal}/connect/token"),
                        Scopes = scopes,
                    }
                }
            };
            swaggerDoc.Components ??= new();
            swaggerDoc.Components.SecuritySchemes.Add("oauth2", securityScheme);
        }
    }

    private class AuthorizationOperationFilter : IOperationFilter
    {
        private readonly string[] _scopes;

        public AuthorizationOperationFilter(string[] scopes)
        {
            _scopes = scopes;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var metadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;

            if (!metadata.OfType<IAuthorizeData>().Any())
            {
                return;
            }

            operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });

            var oAuthScheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
            };

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new()
                {
                    [oAuthScheme] = _scopes
                }
            };
        }
    }

    private class DeprecatedOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Deprecated |= context.ApiDescription.IsDeprecated();
        }
    }
}

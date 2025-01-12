using System.Text;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.OpenApi.Extensions;
using Microsoft.AspNetCore.OpenApi.Transformers;

namespace eShop.ServiceDefaults;

internal static class OpenApiOptionsExtensions
{
    public static SwaggerGenOptions ApplyApiVersionInfo(this SwaggerGenOptions options, string title, string description)
    {
        options.DocumentFilter<ApiVersionInfoFilter>(title, description);
        return options;
    }

    public static SwaggerGenOptions ApplySecuritySchemeDefinitions(this SwaggerGenOptions options)
    {
        options.DocumentFilter<SecuritySchemeDefinitionsFilter>();
        return options;
    }

    public static SwaggerGenOptions ApplyAuthorizationChecks(this SwaggerGenOptions options, string[] scopes)
    {
        options.OperationFilter<AuthorizationCheckFilter>(scopes);
        return options;
    }

    public static SwaggerGenOptions ApplyOperationDeprecatedStatus(this SwaggerGenOptions options)
    {
        options.OperationFilter<OperationDeprecatedStatusFilter>();
        return options;
    }

    private class ApiVersionInfoFilter : IDocumentFilter
    {
        private readonly string _title;
        private readonly string _description;

        public ApiVersionInfoFilter(string title, string description)
        {
            _title = title;
            _description = description;
        }

        public void Apply(OpenApiDocument document, DocumentFilterContext context)
        {
            var versionedDescriptionProvider = context.ServiceProvider.GetService<IApiVersionDescriptionProvider>();
            var apiDescription = versionedDescriptionProvider?.ApiVersionDescriptions
                .SingleOrDefault(description => description.GroupName == document.Info.Version);
            
            if (apiDescription is null)
            {
                return;
            }

            document.Info.Version = apiDescription.ApiVersion.ToString();
            document.Info.Title = _title;
            document.Info.Description = BuildDescription(apiDescription, _description);
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
    }

    private class SecuritySchemeDefinitionsFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument document, DocumentFilterContext context)
        {
            var configuration = context.ServiceProvider.GetService<IConfiguration>();
            var identitySection = configuration?.GetSection("Identity");
            if (!identitySection?.Exists() ?? true)
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
            document.Components ??= new();
            document.Components.SecuritySchemes.Add("oauth2", securityScheme);
        }
    }

    private class AuthorizationCheckFilter : IOperationFilter
    {
        private readonly string[] _scopes;

        public AuthorizationCheckFilter(string[] scopes)
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

    private class OperationDeprecatedStatusFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Deprecated |= context.ApiDescription.IsDeprecated();
        }
    }
}

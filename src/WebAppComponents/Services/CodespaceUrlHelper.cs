using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;

namespace eShop.WebAppComponents.Services;

public class CodespaceUrlHelper(NavigationManager navigationManager, IConfiguration configuration) : ICodespaceUrlHelper
{
    public string GetUriWithQueryParameters(string baseUri, IDictionary<string, object?> parameters)
    {
        var uri = navigationManager.GetUriWithQueryParameters(baseUri, parameters as IReadOnlyDictionary<string, object?> ?? parameters.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
        
        // If not running in Codespaces, return the original URI
        if (!configuration.GetValue<bool>("CODESPACES"))
        {
            return uri;
        }
        
        // If running in Codespaces, check if we need to rewrite localhost URLs
        var parsedUri = new Uri(uri);
        if (parsedUri.Host == "localhost")
        {
            var codespaceName = configuration["CODESPACE_NAME"];
            var gitHubCodespacesPortForwardingDomain = configuration["GITHUB_CODESPACES_PORT_FORWARDING_DOMAIN"];
            
            if (!string.IsNullOrEmpty(codespaceName) && !string.IsNullOrEmpty(gitHubCodespacesPortForwardingDomain))
            {
                return $"{parsedUri.Scheme}://{codespaceName}-{parsedUri.Port}.{gitHubCodespacesPortForwardingDomain}{parsedUri.PathAndQuery}";
            }
        }
        
        return uri;
    }
}
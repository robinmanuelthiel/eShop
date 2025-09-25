namespace eShop.WebAppComponents.Services;

public interface ICodespaceUrlHelper
{
    /// <summary>
    /// Gets a URI with query parameters, handling Codespace URL rewriting when necessary
    /// </summary>
    string GetUriWithQueryParameters(string baseUri, IDictionary<string, object?> parameters);
}
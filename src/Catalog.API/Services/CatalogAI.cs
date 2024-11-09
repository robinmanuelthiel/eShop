using System.Diagnostics;
using Pgvector;

namespace eShop.Catalog.API.Services;

public sealed class CatalogAI : ICatalogAI
{
    /// <summary>The web host environment.</summary>
    private readonly IWebHostEnvironment _environment;
    /// <summary>Logger for use in AI operations.</summary>
    private readonly ILogger _logger;

    public CatalogAI(IWebHostEnvironment environment, ILogger<CatalogAI> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    /// <inheritdoc/>
    public bool IsEnabled => false; // AI features temporarily disabled for .NET 8 migration

    /// <inheritdoc/>
    public ValueTask<Vector> GetEmbeddingAsync(CatalogItem item) =>
        ValueTask.FromResult(new Vector(new float[1024])); // Return empty vector instead of null

    /// <inheritdoc/>
    public ValueTask<IReadOnlyList<Vector>> GetEmbeddingsAsync(IEnumerable<CatalogItem> items) =>
        ValueTask.FromResult<IReadOnlyList<Vector>>(Array.Empty<Vector>()); // Return empty list instead of null

    /// <inheritdoc/>
    public ValueTask<Vector> GetEmbeddingAsync(string text) =>
        ValueTask.FromResult(new Vector(new float[1024])); // Return empty vector instead of null

    private static string CatalogItemToString(CatalogItem item) => $"{item.Name} {item.Description}";
}

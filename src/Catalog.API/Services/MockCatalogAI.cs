using Pgvector;
using eShop.Catalog.API.Model;

namespace eShop.Catalog.API.Services;

public class MockCatalogAI : ICatalogAI
{
    private const int VECTOR_DIMENSIONS = 384; // Changed from 1536 to match PostgreSQL expectation

    public bool IsEnabled => false;

    public ValueTask<Vector> GetEmbeddingAsync(string text)
    {
        // Return a zero vector with correct dimensions for testing purposes
        return ValueTask.FromResult(new Vector(new float[VECTOR_DIMENSIONS]));
    }

    public ValueTask<Vector> GetEmbeddingAsync(CatalogItem item)
    {
        // Return a zero vector with correct dimensions for testing purposes
        return ValueTask.FromResult(new Vector(new float[VECTOR_DIMENSIONS]));
    }

    public ValueTask<IReadOnlyList<Vector>> GetEmbeddingsAsync(IEnumerable<CatalogItem> items)
    {
        // Return zero vectors with correct dimensions for testing purposes
        var vectors = items.Select(_ => new Vector(new float[VECTOR_DIMENSIONS])).ToList();
        return ValueTask.FromResult<IReadOnlyList<Vector>>(vectors);
    }
}

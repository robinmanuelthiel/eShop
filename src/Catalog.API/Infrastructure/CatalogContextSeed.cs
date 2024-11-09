#nullable enable

using System.Text.Json;
using eShop.Catalog.API.Services;
using Pgvector;

namespace eShop.Catalog.API.Infrastructure;

public partial class CatalogContextSeed : IDbSeeder<CatalogContext>
{
    private readonly IWebHostEnvironment _env;
    private readonly IOptions<CatalogOptions> _settings;
    private readonly ICatalogAI? _catalogAI;
    private readonly ILogger<CatalogContextSeed> _logger;

    public CatalogContextSeed(
        IWebHostEnvironment env,
        IOptions<CatalogOptions> settings,
        ILogger<CatalogContextSeed> logger,
        ICatalogAI? catalogAI = null)
    {
        _env = env;
        _settings = settings;
        _catalogAI = catalogAI;
        _logger = logger;
    }

    public async Task SeedAsync(CatalogContext context)
    {
        var useCustomizationData = _settings.Value.UseCustomizationData;
        var contentRootPath = _env.ContentRootPath;
        var picturePath = _env.WebRootPath;

        // Workaround from https://github.com/npgsql/efcore.pg/issues/292#issuecomment-388608426
        context.Database.OpenConnection();
        ((NpgsqlConnection)context.Database.GetDbConnection()).ReloadTypes();

        if (!context.CatalogItems.Any())
        {
            var sourcePath = Path.Combine(contentRootPath, "Setup", "catalog.json");
            var sourceJson = File.ReadAllText(sourcePath);
            var sourceItems = JsonSerializer.Deserialize<CatalogSourceEntry[]>(sourceJson)
                ?? throw new InvalidOperationException("Failed to deserialize catalog.json");

            context.CatalogBrands.RemoveRange(context.CatalogBrands);
            await context.CatalogBrands.AddRangeAsync(sourceItems.Select(x => x.Brand).Distinct()
                .Select(brandName => new CatalogBrand { Brand = brandName }));
            _logger.LogInformation("Seeded catalog with {NumBrands} brands", context.CatalogBrands.Count());

            context.CatalogTypes.RemoveRange(context.CatalogTypes);
            await context.CatalogTypes.AddRangeAsync(sourceItems.Select(x => x.Type).Distinct()
                .Select(typeName => new CatalogType { Type = typeName }));
            _logger.LogInformation("Seeded catalog with {NumTypes} types", context.CatalogTypes.Count());

            await context.SaveChangesAsync();

            var brandIdsByName = await context.CatalogBrands.ToDictionaryAsync(x => x.Brand, x => x.Id);
            var typeIdsByName = await context.CatalogTypes.ToDictionaryAsync(x => x.Type, x => x.Id);

            var catalogItems = sourceItems.Select(source => new CatalogItem
            {
                Id = source.Id,
                Name = source.Name,
                Description = source.Description,
                Price = source.Price,
                CatalogBrandId = brandIdsByName[source.Brand],
                CatalogTypeId = typeIdsByName[source.Type],
                CatalogBrand = context.CatalogBrands.First(b => b.Id == brandIdsByName[source.Brand]),
                CatalogType = context.CatalogTypes.First(t => t.Id == typeIdsByName[source.Type]),
                AvailableStock = 100,
                MaxStockThreshold = 200,
                RestockThreshold = 10,
                PictureFileName = $"{source.Id}.webp",
            }).ToArray();

            if (_catalogAI?.IsEnabled == true)
            {
                _logger.LogInformation("Generating {NumItems} embeddings", catalogItems.Length);
                IReadOnlyList<Vector> embeddings = await _catalogAI.GetEmbeddingsAsync(catalogItems);
                for (int i = 0; i < catalogItems.Length; i++)
                {
                    catalogItems[i].Embedding = embeddings[i];
                }
            }

            await context.CatalogItems.AddRangeAsync(catalogItems);
            _logger.LogInformation("Seeded catalog with {NumItems} items", context.CatalogItems.Count());
            await context.SaveChangesAsync();
        }
    }

    private sealed class CatalogSourceEntry
    {
        public required int Id { get; set; }
        public required string Type { get; set; }
        public required string Brand { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required decimal Price { get; set; }
    }
}

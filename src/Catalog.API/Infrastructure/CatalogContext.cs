namespace eShop.Catalog.API.Infrastructure;

/// <remarks>
/// Add migrations using the following command inside the 'Catalog.API' project directory:
///
/// dotnet ef migrations add --context CatalogContext [migration-name]
/// </remarks>
public class CatalogContext : DbContext
{
    private readonly IConfiguration _configuration;

    public CatalogContext(DbContextOptions<CatalogContext> options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }

    public DbSet<CatalogItem> CatalogItems { get; set; }
    public DbSet<CatalogBrand> CatalogBrands { get; set; }
    public DbSet<CatalogType> CatalogTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Only enable vector extension if AI features are enabled
        var aiEnabled = _configuration.GetSection("CatalogOptions:EnableAIFeatures").Get<bool>();
        if (aiEnabled)
        {
            builder.HasPostgresExtension("vector");
        }

        builder.ApplyConfiguration(new CatalogBrandEntityTypeConfiguration());
        builder.ApplyConfiguration(new CatalogTypeEntityTypeConfiguration());
        builder.ApplyConfiguration(new CatalogItemEntityTypeConfiguration(aiEnabled));

        // Add the outbox table to this context
        builder.UseIntegrationEventLogs();
    }
}

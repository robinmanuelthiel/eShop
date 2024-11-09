namespace eShop.Catalog.API.Infrastructure.EntityConfigurations;

class CatalogItemEntityTypeConfiguration
    : IEntityTypeConfiguration<CatalogItem>
{
    private readonly bool _aiEnabled;

    public CatalogItemEntityTypeConfiguration(bool aiEnabled = false)
    {
        _aiEnabled = aiEnabled;
    }

    public void Configure(EntityTypeBuilder<CatalogItem> builder)
    {
        builder.ToTable("Catalog");

        builder.Property(ci => ci.Name)
            .HasMaxLength(50);

        if (_aiEnabled)
        {
            builder.Property(ci => ci.Embedding)
                .HasColumnType("vector(384)");
        }

        builder.HasOne(ci => ci.CatalogBrand)
            .WithMany();

        builder.HasOne(ci => ci.CatalogType)
            .WithMany();

        builder.HasIndex(ci => ci.Name);
    }
}

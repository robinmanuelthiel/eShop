using System.Text.Json.Serialization;
using Pgvector;

namespace eShop.Catalog.API.Model;

public partial class CatalogItem
{
    /// <summary>Optional embedding for the catalog item's description.</summary>
    [JsonIgnore]
    public Vector? Embedding { get; set; }
}

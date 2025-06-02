namespace eShop.Basket.API.Model;

/// <summary>
/// Repräsentiert einen einzelnen Artikel im Warenkorb.
/// </summary>
public class BasketItem : IValidatableObject
{
    /// <summary>
    /// Eindeutige ID des Warenkorb-Artikels.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Produkt-ID des Artikels.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Name des Produkts.
    /// </summary>
    public string ProductName { get; set; }

    /// <summary>
    /// Aktueller Einzelpreis des Produkts.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Vorheriger Einzelpreis des Produkts.
    /// </summary>
    public decimal OldUnitPrice { get; set; }

    /// <summary>
    /// Anzahl der Einheiten dieses Produkts im Warenkorb.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// URL zum Produktbild.
    /// </summary>
    public string PictureUrl { get; set; }

    /// <summary>
    /// Validiert die Eigenschaften des Warenkorb-Artikels.
    /// </summary>
    /// <param name="validationContext">Der Validierungskontext.</param>
    /// <returns>Eine Auflistung von Validierungsergebnissen.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (Quantity < 1)
        {
            results.Add(new ValidationResult("Invalid number of units", new[] { "Quantity" }));
        }

        return results;
    }
}

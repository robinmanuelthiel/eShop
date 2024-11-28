namespace eShop.Basket.API.Model;

public class BasketItem : IValidatableObject
{
    /// <summary>
    /// Eindeutige Kennung des Warenkorbartikels.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Produktkennung.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Name des Produkts.
    /// </summary>
    public string ProductName { get; set; }

    /// <summary>
    /// Aktueller Stückpreis des Produkts.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Alter Stückpreis des Produkts.
    /// </summary>
    public decimal OldUnitPrice { get; set; }

    /// <summary>
    /// Menge des Produkts.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// URL des Produktbildes.
    /// </summary>
    public string PictureUrl { get; set; }

    /// <summary>
    /// Validiert den Warenkorbartikel.
    /// </summary>
    /// <param name="validationContext">Der Kontext, in dem die Validierung durchgeführt wird.</param>
    /// <returns>Eine Sammlung von Validierungsergebnissen.</returns>
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

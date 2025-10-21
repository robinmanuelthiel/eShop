using System.ComponentModel.DataAnnotations;

namespace eShop.Basket.API.Model;

/// <summary>
/// Represents an item in a shopping basket containing product information, pricing, and quantity details.
/// Implements IValidatableObject to provide custom validation logic for basket item properties.
/// </summary>
/// <remarks>
/// This class is used to store individual product items within a customer's shopping basket,
/// including current and previous pricing information for comparison purposes.
/// </remarks>
public class BasketItem : IValidatableObject
{
    [Required(ErrorMessage = "Die Produkt-ID ist erforderlich.")]
    [StringLength(50, ErrorMessage = "Die ID darf maximal 50 Zeichen lang sein.")]
    public string Id { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Die Produkt-ID muss größer als 0 sein.")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Der Produktname ist erforderlich.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Der Produktname muss zwischen 1 und 100 Zeichen lang sein.")]
    public string ProductName { get; set; } = string.Empty;

    [Range(0.01, 999999.99, ErrorMessage = "Der Stückpreis muss zwischen 0,01 und 999.999,99 liegen.")]
    [DataType(DataType.Currency)]
    public decimal UnitPrice { get; set; }

    [Range(0, 999999.99, ErrorMessage = "Der alte Stückpreis muss zwischen 0 und 999.999,99 liegen.")]
    [DataType(DataType.Currency)]
    public decimal OldUnitPrice { get; set; }

    [Range(1, 1000, ErrorMessage = "Die Menge muss zwischen 1 und 1000 liegen.")]
    public int Quantity { get; set; }

    [Url(ErrorMessage = "Die Bild-URL ist nicht gültig.")]
    [StringLength(500, ErrorMessage = "Die Bild-URL darf maximal 500 Zeichen lang sein.")]
    public string PictureUrl { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        // Erweiterte Validierung für Quantity
        if (Quantity < 1)
        {
            results.Add(new ValidationResult("Die Menge muss mindestens 1 betragen.", new[] { nameof(Quantity) }));
        }

        if (Quantity > 1000)
        {
            results.Add(new ValidationResult("Die Menge darf maximal 1000 betragen.", new[] { nameof(Quantity) }));
        }

        // Validierung für ProductId
        if (ProductId <= 0)
        {
            results.Add(new ValidationResult("Die Produkt-ID muss größer als 0 sein.", new[] { nameof(ProductId) }));
        }

        // Validierung für UnitPrice
        if (UnitPrice <= 0)
        {
            results.Add(new ValidationResult("Der Stückpreis muss größer als 0 sein.", new[] { nameof(UnitPrice) }));
        }

        // Validierung für OldUnitPrice (darf 0 sein, aber nicht negativ)
        if (OldUnitPrice < 0)
        {
            results.Add(new ValidationResult("Der alte Stückpreis darf nicht negativ sein.", new[] { nameof(OldUnitPrice) }));
        }

        // Geschäftslogik-Validierung: Prüfung auf verdächtige Preisänderungen
        if (OldUnitPrice > 0 && UnitPrice > 0)
        {
            var priceChangePercentage = Math.Abs((UnitPrice - OldUnitPrice) / OldUnitPrice) * 100;
            if (priceChangePercentage > 50)
            {
                results.Add(new ValidationResult(
                    "Die Preisänderung ist ungewöhnlich hoch (über 50%). Bitte überprüfen Sie den Preis.",
                    new[] { nameof(UnitPrice), nameof(OldUnitPrice) }));
            }
        }

        // Validierung für PictureUrl (falls angegeben)
        if (!string.IsNullOrEmpty(PictureUrl))
        {
            if (!Uri.TryCreate(PictureUrl, UriKind.Absolute, out var uri))
            {
                results.Add(new ValidationResult("Die Bild-URL ist nicht gültig.", new[] { nameof(PictureUrl) }));
            }
            else if (uri.Scheme != "http" && uri.Scheme != "https")
            {
                results.Add(new ValidationResult("Die Bild-URL muss HTTP oder HTTPS verwenden.", new[] { nameof(PictureUrl) }));
            }
        }

        // Validierung für ProductName (zusätzliche Geschäftslogik)
        if (!string.IsNullOrWhiteSpace(ProductName))
        {
            if (ProductName.Trim().Length != ProductName.Length)
            {
                results.Add(new ValidationResult("Der Produktname darf nicht mit Leerzeichen beginnen oder enden.", new[] { nameof(ProductName) }));
            }

            // Prüfung auf verdächtige Zeichen
            var suspiciousChars = new[] { '<', '>', '"', '\'', '&' };
            if (ProductName.Any(c => suspiciousChars.Contains(c)))
            {
                results.Add(new ValidationResult("Der Produktname enthält ungültige Zeichen.", new[] { nameof(ProductName) }));
            }
        }

        return results;
    }
}

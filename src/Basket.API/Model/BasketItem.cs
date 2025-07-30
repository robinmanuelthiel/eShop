namespace eShop.Basket.API.Model;

public class BasketItem : IValidatableObject
{
    public string Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal OldUnitPrice { get; set; }
    public int Quantity { get; set; }
    public string PictureUrl { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (Quantity < 1)
        {
            results.Add(new ValidationResult("🤌 Numero di unità non valido", new[] { "Quantity" }));
        }

        // Validate the URL if it's a valid URL format
        if (!string.IsNullOrEmpty(PictureUrl) && !Uri.IsWellFormedUriString(PictureUrl, UriKind.Absolute))
        {
            results.Add(new ValidationResult("🤌 Formato URL non valido", new[] { "PictureUrl" }));
        }

        return results;
    }
}

namespace eShop.Basket.API.Model;

public class BasketItem : IValidatableObject
{
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the product.
    /// </summary>
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
            results.Add(new ValidationResult("Invalid number of units", new[] { "Quantity" }));
        }

        // Make sure, that the old unit price is not higher than the new unit price
        if (OldUnitPrice > UnitPrice)
        {
            results.Add(new ValidationResult("Old unit price cannot be higher than the new unit price", new[] { "OldUnitPrice" }));
        }

        return results;
    }
}

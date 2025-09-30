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

    /// <summary>
    /// Validates the <see cref="BasketItem"/> instance to ensure that the quantity is at least 1.
    /// </summary>
    /// <param name="validationContext">The context information about the validation operation.</param>
    /// <returns>
    /// An <see cref="IEnumerable{ValidationResult}"/> containing validation errors, if any.
    /// Returns a validation error if <c>Quantity</c> is less than 1.
    /// </returns>
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

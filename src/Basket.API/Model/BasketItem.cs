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
    /// Validates the properties of the basket item.
    /// </summary>
    /// <param name="validationContext">The context information about the validation operation.</param>
    /// <returns>
    /// An <see cref="IEnumerable{ValidationResult}"/> containing any validation errors found.
    /// </returns>
    /// <remarks>
    /// Validation checks performed:
    /// <list type="bullet">
    /// <item>
    /// <description>Ensures <c>Quantity</c> is at least 1. // Checks for valid quantity</description>
    /// </item>
    /// <item>
    /// <description>Ensures <c>UnitPrice</c> is not negative. // Checks for valid unit price</description>
    /// </item>
    /// <item>
    /// <description>Ensures <c>ProductName</c> is not null or empty. // Checks for valid product name</description>
    /// </item>
    /// <item>
    /// <description>Ensures <c>ProductId</c> is not negative. // Checks for valid product id</description>
    /// </item>
    /// </list>
    /// </remarks>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (Quantity < 1)
        {
            results.Add(new ValidationResult("Invalid number of units", new[] { "Quantity" }));
        }

        if (UnitPrice < 0)
        {
            results.Add(new ValidationResult("Invalid unit price", new[] { "UnitPrice" }));
        }

        if (string.IsNullOrEmpty(ProductName))
        {
            results.Add(new ValidationResult("Invalid product name", new[] { "ProductName" }));
        }

        if (ProductId < 0)
        {
            results.Add(new ValidationResult("Invalid product id", new[] { "ProductId" }));
        }
        return results;
    }
}

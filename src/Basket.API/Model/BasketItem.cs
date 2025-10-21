namespace eShop.Basket.API.Model;

public class BasketItem : IValidatableObject
/// <summary>
/// Represents an item in a shopping basket containing product information, pricing, and quantity details.
/// Implements IValidatableObject to provide custom validation logic for basket item properties.
/// </summary>
/// <remarks>
/// This class is used to store individual product items within a customer's shopping basket,
/// including current and previous pricing information for comparison purposes.
/// </remarks>
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
            results.Add(new ValidationResult("Invalid number of units", new[] { "Quantity" }));
        }

        return results;
    }
}

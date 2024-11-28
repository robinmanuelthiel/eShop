namespace eShop.Basket.API.Model;

public class CustomerBasket : IValidatableObject
{
    private const int MaxItemsPerCart = 20;

    public string BuyerId { get; set; }

    public List<BasketItem> Items { get; set; } = [];

    public CustomerBasket() { }

    public CustomerBasket(string customerId)
    {
        BuyerId = customerId;
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (Items.Count > MaxItemsPerCart)
        {
            results.Add(new ValidationResult($"Exceeds maximum number of {MaxItemsPerCart} different items", new[] { "Items" }));
        }

        return results;
    }
}

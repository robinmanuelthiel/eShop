namespace eShop.Basket.API.Model;

/// <summary>
/// Represents a shopping basket for a specific customer, containing a collection of basket items.
/// </summary>
public class CustomerBasket
{
    /// <summary>
    /// Gets or sets the unique identifier of the buyer associated with this basket.
    /// </summary>
    public string BuyerId { get; set; }

    /// <summary>
    /// Gets or sets the list of items in the customer's basket.
    /// Initialized as an empty list by default.
    /// </summary>
    public List<BasketItem> Items { get; set; } = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerBasket"/> class.
    /// </summary>
    public CustomerBasket() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerBasket"/> class
    /// with the specified customer identifier.
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer.</param>
    public CustomerBasket(string customerId)
    {
        BuyerId = customerId;
    }
}

namespace eShop.Ordering.API.Application.Models;

public class BasketItem
{
    public string Id { get; init; }


    /// <summary>
    /// Gets the unique identifier of the product.
    /// </summary>
    public int ProductId { get; init; }
    public string ProductName { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal OldUnitPrice { get; init; }
    public int Quantity { get; init; }
    public string PictureUrl { get; init; }
}


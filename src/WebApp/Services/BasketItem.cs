namespace eShop.WebApp.Services;

public class BasketItem
{
    public required string Id { get; set; }
    public int ProductId { get; set; }
    public required string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal OldUnitPrice { get; set; }
    public int Quantity { get; set; }

    public string CamelCaseProductName
    {
        get
        {
            if (string.IsNullOrWhiteSpace(ProductName))
                return string.Empty;

            var words = ProductName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var result = string.Concat(words.Select(w => char.ToUpperInvariant(w[0]) + w.Substring(1)));
            return result;
        }
    }
}

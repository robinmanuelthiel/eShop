using eShop.Basket.API.Grpc;
using GrpcBasketItem = eShop.Basket.API.Grpc.BasketItem;
using GrpcBasketClient = eShop.Basket.API.Grpc.Basket.BasketClient;

namespace eShop.WebApp.Services;

public class BasketService(GrpcBasketClient basketClient)
{
    /// <summary>
    /// Retrieves the current basket asynchronously.
    /// </summary>
    /// <returns>A read-only collection of basket quantities.</returns>
    public async Task<IReadOnlyCollection<BasketQuantity>> GetBasketAsync()
    {
        var result = await basketClient.GetBasketAsync(new ());
        return MapToBasket(result);
    }

    /// <summary>
    /// Deletes the current basket asynchronously.
    /// </summary>
    public async Task DeleteBasketAsync()
    {
        await basketClient.DeleteBasketAsync(new DeleteBasketRequest());
    }

    /// <summary>
    /// Updates the basket with the provided items asynchronously.
    /// </summary>
    /// <param name="basket">A read-only collection of basket quantities to update.</param>
    public async Task UpdateBasketAsync(IReadOnlyCollection<BasketQuantity> basket)
    {
        var updatePayload = new UpdateBasketRequest();

        foreach (var item in basket)
        {
            var updateItem = new GrpcBasketItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
            };
            updatePayload.Items.Add(updateItem);
        }

        await basketClient.UpdateBasketAsync(updatePayload);
    }

    /// <summary>
    /// Maps the gRPC response to a list of basket quantities.
    /// </summary>
    /// <param name="response">The gRPC response containing basket items.</param>
    /// <returns>A list of basket quantities.</returns>
    private static List<BasketQuantity> MapToBasket(CustomerBasketResponse response)
    {
        var result = new List<BasketQuantity>();
        foreach (var item in response.Items)
        {
            result.Add(new BasketQuantity(item.ProductId, item.Quantity));
        }

        return result;
    }
}

public record BasketQuantity(int ProductId, int Quantity);

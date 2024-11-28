using System.Diagnostics.CodeAnalysis;
using eShop.Basket.API.Repositories;
using eShop.Basket.API.Extensions;
using eShop.Basket.API.Model;

namespace eShop.Basket.API.Grpc;

public class BasketService(
    IBasketRepository repository,
    ILogger<BasketService> logger) : Basket.BasketBase
{
    /// <summary>
    /// Retrieves the basket for the authenticated user.
    /// </summary>
    /// <param name="request">The request containing the basket ID.</param>
    /// <param name="context">The server call context.</param>
    /// <returns>A task representing the asynchronous operation, with a CustomerBasketResponse as result.</returns>
    [AllowAnonymous]
    public override async Task<CustomerBasketResponse> GetBasket(GetBasketRequest request, ServerCallContext context)
    {
        var userId = context.GetUserIdentity();
        if (string.IsNullOrEmpty(userId))
        {
            return new();
        }

        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug("Begin GetBasketById call from method {Method} for basket id {Id}", context.Method, userId);
        }

        var data = await repository.GetBasketAsync(userId);

        if (data is not null)
        {
            return MapToCustomerBasketResponse(data);
        }

        return new();
    }

    /// <summary>
    /// Updates the basket for the authenticated user.
    /// </summary>
    /// <param name="request">The request containing the updated basket information.</param>
    /// <param name="context">The server call context.</param>
    /// <returns>A task representing the asynchronous operation, with a CustomerBasketResponse as result.</returns>
    public override async Task<CustomerBasketResponse> UpdateBasket(UpdateBasketRequest request, ServerCallContext context)
    {
        var userId = context.GetUserIdentity();
        if (string.IsNullOrEmpty(userId))
        {
            ThrowNotAuthenticated();
        }

        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug("Begin UpdateBasket call from method {Method} for basket id {Id}", context.Method, userId);
        }

        var customerBasket = MapToCustomerBasket(userId, request);
        var response = await repository.UpdateBasketAsync(customerBasket);
        if (response is null)
        {
            ThrowBasketDoesNotExist(userId);
        }

        return MapToCustomerBasketResponse(response);
    }

    /// <summary>
    /// Deletes the basket for the authenticated user.
    /// </summary>
    /// <param name="request">The request containing the basket ID to delete.</param>
    /// <param name="context">The server call context.</param>
    /// <returns>A task representing the asynchronous operation, with a DeleteBasketResponse as result.</returns>
    public override async Task<DeleteBasketResponse> DeleteBasket(DeleteBasketRequest request, ServerCallContext context)
    {
        var userId = context.GetUserIdentity();
        if (string.IsNullOrEmpty(userId))
        {
            ThrowNotAuthenticated();
        }

        await repository.DeleteBasketAsync(userId);
        return new();
    }

    /// <summary>
    /// Throws an exception indicating that the caller is not authenticated.
    /// </summary>
    [DoesNotReturn]
    private static void ThrowNotAuthenticated() => throw new RpcException(new Status(StatusCode.Unauthenticated, "The caller is not authenticated."));

    /// <summary>
    /// Throws an exception indicating that the basket does not exist.
    /// </summary>
    /// <param name="userId">The ID of the user whose basket does not exist.</param>
    [DoesNotReturn]
    private static void ThrowBasketDoesNotExist(string userId) => throw new RpcException(new Status(StatusCode.NotFound, $"Basket with buyer id {userId} does not exist"));

    /// <summary>
    /// Maps a CustomerBasket object to a CustomerBasketResponse object.
    /// </summary>
    /// <param name="customerBasket">The CustomerBasket object to map.</param>
    /// <returns>A CustomerBasketResponse object.</returns>
    private static CustomerBasketResponse MapToCustomerBasketResponse(CustomerBasket customerBasket)
    {
        var response = new CustomerBasketResponse();

        foreach (var item in customerBasket.Items)
        {
            response.Items.Add(new BasketItem()
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
            });
        }

        return response;
    }

    /// <summary>
    /// Maps an UpdateBasketRequest object to a CustomerBasket object.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="customerBasketRequest">The UpdateBasketRequest object to map.</param>
    /// <returns>A CustomerBasket object.</returns>
    private static CustomerBasket MapToCustomerBasket(string userId, UpdateBasketRequest customerBasketRequest)
    {
        var response = new CustomerBasket
        {
            BuyerId = userId
        };

        foreach (var item in customerBasketRequest.Items)
        {
            response.Items.Add(new()
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
            });
        }

        return response;
    }
}

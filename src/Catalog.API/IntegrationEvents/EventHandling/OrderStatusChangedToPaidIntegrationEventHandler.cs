namespace eShop.Catalog.API.IntegrationEvents.EventHandling;

public class OrderStatusChangedToPaidIntegrationEventHandler(
    CatalogContext catalogContext,
    ILogger<OrderStatusChangedToPaidIntegrationEventHandler> logger) :
    IIntegrationEventHandler<OrderStatusChangedToPaidIntegrationEvent>
{
    public async Task Handle(OrderStatusChangedToPaidIntegrationEvent @event)
    {
        logger.LogInformation("Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);

        //we're not blocking stock/inventory
        foreach (var orderStockItem in @event.OrderStockItems)
        {
            var catalogItem = catalogContext.CatalogItems.Find(orderStockItem.ProductId);

            if (catalogItem is null)
            {
                logger.LogWarning("Product {ProductId} not found while handling paid order {OrderId}",
                    orderStockItem.ProductId, @event.OrderId);
                continue;
            }

            catalogItem.RemoveStock(orderStockItem.Units);
        }

        await catalogContext.SaveChangesAsync();
    }
}

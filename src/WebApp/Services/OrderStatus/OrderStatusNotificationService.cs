/// <summary>
/// Provides services related to order status notifications within the eShop web application.
/// </summary>
/// <remarks>
/// Siehe die zugehörige Mermaid-Diagrammdatei für eine visuelle Darstellung der Klasse.
/// Die Diagrammdatei befindet sich parallel zu dieser C#-Datei und trägt die Endung ".mmd".
/// </remarks>
namespace eShop.WebApp.Services;

public class OrderStatusNotificationService
{
    // Locking manually because we need multiple values per key, and only need to lock very briefly
    private readonly object _subscriptionsLock = new();
    private readonly Dictionary<string, HashSet<Subscription>> _subscriptionsByBuyerId = new();

    public IDisposable SubscribeToOrderStatusNotifications(string buyerId, Func<Task> callback)
    {
        var subscription = new Subscription(this, buyerId, callback);

        lock (_subscriptionsLock)
        {
            if (!_subscriptionsByBuyerId.TryGetValue(buyerId, out var subscriptions))
            {
                subscriptions = [];
                _subscriptionsByBuyerId.Add(buyerId, subscriptions);
            }

            subscriptions.Add(subscription);
        }

        return subscription;
    }

    public Task NotifyOrderStatusChangedAsync(string buyerId)
    {
        lock (_subscriptionsLock)
        {
            return _subscriptionsByBuyerId.TryGetValue(buyerId, out var subscriptions)
                ? Task.WhenAll(subscriptions.Select(s => s.NotifyAsync()))
                : Task.CompletedTask;
        }
    }

    private void Unsubscribe(string buyerId, Subscription subscription)
    {
        lock (_subscriptionsLock)
        {
            if (_subscriptionsByBuyerId.TryGetValue(buyerId, out var subscriptions))
            {
                subscriptions.Remove(subscription);
                if (subscriptions.Count == 0)
                {
                    _subscriptionsByBuyerId.Remove(buyerId);
                }
            }
        }
    }

    private class Subscription(OrderStatusNotificationService owner, string buyerId, Func<Task> callback) : IDisposable
    {
        public Task NotifyAsync()
        {
            return callback();
        }

        public void Dispose()
            => owner.Unsubscribe(buyerId, this);
    }
}

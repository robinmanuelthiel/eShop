namespace eShop.OrderProcessor;

/// <summary>
/// Represents configuration options for background tasks.
/// </summary>
public class BackgroundTaskOptions
{
    /// <summary>
    /// Gets or sets the grace period time, in seconds, to wait before performing a background task.
    /// </summary>
    public int GracePeriodTime { get; set; }

    /// <summary>
    /// Gets or sets the interval time, in seconds, to check for updates in the background task.
    /// </summary>
    public int CheckUpdateTime { get; set; }
}

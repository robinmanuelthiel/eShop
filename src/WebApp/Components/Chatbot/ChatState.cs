using System.ComponentModel;
using System.Security.Claims;
using System.Text.Json;
using eShop.WebAppComponents.Services;

namespace eShop.WebApp.Chatbot;

public class ChatState
{
    private readonly ICatalogService _catalogService;
    private readonly IBasketState _basketState;
    private readonly ClaimsPrincipal _user;
    private readonly ILogger _logger;
    private readonly IProductImageUrlProvider _productImages;

    public ChatState(
        ICatalogService catalogService,
        IBasketState basketState,
        ClaimsPrincipal user,
        IProductImageUrlProvider productImages,
        ILoggerFactory loggerFactory)
    {
        _catalogService = catalogService;
        _basketState = basketState;
        _user = user;
        _productImages = productImages;
        _logger = loggerFactory.CreateLogger(typeof(ChatState));
    }

    // Temporary simplified chat implementation for .NET 8 migration
    public IList<string> Messages { get; } = new List<string>
    {
        "Chat functionality is temporarily unavailable during system upgrade."
    };

    public Task AddUserMessageAsync(string userText, Action onMessageAdded)
    {
        Messages.Add($"User: {userText}");
        Messages.Add("System: Chat functionality is temporarily unavailable during system upgrade. Please try again later.");
        onMessageAdded();
        return Task.CompletedTask;
    }

    private string Error(Exception e, string message)
    {
        if (_logger.IsEnabled(LogLevel.Error))
        {
            _logger.LogError(e, message);
        }
        return message;
    }
}

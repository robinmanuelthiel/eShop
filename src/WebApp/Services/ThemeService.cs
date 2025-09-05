using Microsoft.JSInterop;

namespace eShop.WebApp.Services;

public class ThemeService(IJSRuntime jsRuntime)
{
    public string CurrentTheme { get; private set; } = "light";

    public event Action? OnThemeChanged;

    public async Task ToggleTheme()
    {
        CurrentTheme = CurrentTheme == "light" ? "dark" : "light";
        await jsRuntime.InvokeVoidAsync("setTheme", CurrentTheme);
        OnThemeChanged?.Invoke();
    }
}

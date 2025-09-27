using Microsoft.JSInterop;

namespace eShop.WebApp.Services;

public class ThemeService : IThemeService
{
    private readonly IJSRuntime _jsRuntime;
    private bool _isDarkMode = false;

    public ThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public bool IsDarkMode => _isDarkMode;

    public event Action? ThemeChanged;

    public async Task InitializeAsync()
    {
        try
        {
            // Check if dark mode is saved in localStorage
            var savedTheme = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "eshop-theme");
            _isDarkMode = savedTheme == "dark";
            await ApplyThemeAsync();
        }
        catch
        {
            // Fallback to light mode if localStorage is not available
            _isDarkMode = false;
        }
    }

    public async Task ToggleThemeAsync()
    {
        _isDarkMode = !_isDarkMode;
        await ApplyThemeAsync();
        ThemeChanged?.Invoke();
    }

    private async Task ApplyThemeAsync()
    {
        try
        {
            // Save theme preference to localStorage
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "eshop-theme", _isDarkMode ? "dark" : "light");
            
            // Apply theme class to document body
            var themeClass = _isDarkMode ? "dark-theme" : "light-theme";
            await _jsRuntime.InvokeVoidAsync("eval", $"document.body.className = '{themeClass}';");
        }
        catch
        {
            // Ignore JS errors during theme application
        }
    }
}
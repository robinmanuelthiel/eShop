namespace eShop.WebApp.Services;

public interface IThemeService
{
    bool IsDarkMode { get; }
    event Action? ThemeChanged;
    Task ToggleThemeAsync();
    Task InitializeAsync();
}
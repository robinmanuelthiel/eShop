namespace eShop.WebApp.Services;

public class PinkPricesState
{
    private bool _isPinkPricesEnabled;

    public bool IsPinkPricesEnabled
    {
        get => _isPinkPricesEnabled;
        set
        {
            if (_isPinkPricesEnabled != value)
            {
                _isPinkPricesEnabled = value;
                OnChange?.Invoke();
            }
        }
    }

    public event Action? OnChange;
}

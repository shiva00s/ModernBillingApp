namespace ModernBillingApp.Services
{
    // Theme Management Service (Dark/Light Mode)
    public class ThemeService
    {
        private string _currentTheme = "light";

        public event Action? OnThemeChanged;

        public string CurrentTheme
        {
            get => _currentTheme;
            set
            {
                if (_currentTheme != value)
                {
                    _currentTheme = value;
                    OnThemeChanged?.Invoke();
                }
            }
        }

        public bool IsDarkMode => _currentTheme == "dark";

        public void ToggleTheme()
        {
            CurrentTheme = IsDarkMode ? "light" : "dark";
        }

        public void SetTheme(string theme)
        {
            if (theme == "dark" || theme == "light")
            {
                CurrentTheme = theme;
            }
        }
    }
}


using Microsoft.AspNetCore.Components;
using NewModernBillingApp.Services;

namespace NewModernBillingApp.Components
{
    public partial class App : ComponentBase, IDisposable
    {
        [Inject] private ThemeService themeService { get; set; } = null!;

        protected override void OnInitialized()
        {
            themeService.ThemeChanged += OnThemeChanged;
        }

        private void OnThemeChanged(string theme)
        {
            InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            themeService.ThemeChanged -= OnThemeChanged;
        }
    }
}

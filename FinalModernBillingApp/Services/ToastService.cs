using Microsoft.JSInterop;

namespace ModernBillingApp.Services
{
    public class ToastService
    {
        private readonly IJSRuntime _jsRuntime;

        public ToastService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task ShowSuccess(string message, string title = "Success")
        {
            await _jsRuntime.InvokeVoidAsync("showToast", "success", title, message);
        }

        public async Task ShowError(string message, string title = "Error")
        {
            await _jsRuntime.InvokeVoidAsync("showToast", "error", title, message);
        }

        public async Task ShowInfo(string message, string title = "Info")
        {
            await _jsRuntime.InvokeVoidAsync("showToast", "info", title, message);
        }

        public async Task ShowWarning(string message, string title = "Warning")
        {
            await _jsRuntime.InvokeVoidAsync("showToast", "warning", title, message);
        }
    }
}

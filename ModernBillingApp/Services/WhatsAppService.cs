using System.Text;
using System.Text.Json;

namespace ModernBillingApp.Services
{
    // WhatsApp Integration Service
    // Supports multiple WhatsApp API providers
    public class WhatsAppService
    {
        private readonly HttpClient _httpClient;
        private readonly string? _apiKey;
        private readonly string? _apiUrl;
        private readonly string? _phoneNumberId;

        public WhatsAppService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiKey = configuration["WhatsApp:ApiKey"];
            _apiUrl = configuration["WhatsApp:ApiUrl"] ?? "https://api.whatsapp.com";
            _phoneNumberId = configuration["WhatsApp:PhoneNumberId"];
        }

        // Send text message
        public async Task<bool> SendMessage(string toPhoneNumber, string message)
        {
            if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_phoneNumberId))
            {
                // WhatsApp not configured
                return false;
            }

            try
            {
                var payload = new
                {
                    messaging_product = "whatsapp",
                    to = toPhoneNumber,
                    type = "text",
                    text = new { body = message }
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                var url = $"{_apiUrl}/v1/{_phoneNumberId}/messages";
                var response = await _httpClient.PostAsync(url, content);

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // Send bill receipt via WhatsApp
        public async Task<bool> SendBillReceipt(string toPhoneNumber, string billNo, decimal totalAmount, string? customerName = null)
        {
            var message = $"*Bill Receipt*\n\n" +
                         $"Bill No: {billNo}\n" +
                         $"{(string.IsNullOrEmpty(customerName) ? "" : $"Customer: {customerName}\n")}" +
                         $"Total Amount: ₹{totalAmount:N2}\n\n" +
                         $"Thank you for your purchase!";

            return await SendMessage(toPhoneNumber, message);
        }

        // Send payment reminder
        public async Task<bool> SendPaymentReminder(string toPhoneNumber, string customerName, decimal outstandingAmount, string? billNo = null)
        {
            var message = $"*Payment Reminder*\n\n" +
                         $"Dear {customerName},\n\n" +
                         $"Your outstanding balance is ₹{outstandingAmount:N2}\n" +
                         $"{(string.IsNullOrEmpty(billNo) ? "" : $"Bill No: {billNo}\n")}" +
                         $"Please make payment at your earliest convenience.\n\n" +
                         $"Thank you!";

            return await SendMessage(toPhoneNumber, message);
        }

        // Send low stock alert to admin
        public async Task<bool> SendStockAlert(string adminPhoneNumber, string productName, double currentStock)
        {
            var message = $"*Stock Alert*\n\n" +
                         $"Product: {productName}\n" +
                         $"Current Stock: {currentStock}\n" +
                         $"Please restock soon!";

            return await SendMessage(adminPhoneNumber, message);
        }

        // Send expiry alert
        public async Task<bool> SendExpiryAlert(string adminPhoneNumber, string productName, DateTime expiryDate, int daysUntilExpiry)
        {
            var message = $"*Expiry Alert*\n\n" +
                         $"Product: {productName}\n" +
                         $"Expiry Date: {expiryDate:dd MMM yyyy}\n" +
                         $"Days Remaining: {daysUntilExpiry}\n" +
                         $"Please take action!";

            return await SendMessage(adminPhoneNumber, message);
        }
    }
}


using System.Globalization;

namespace ModernBillingApp.Helpers
{
    // Helper for Indian currency formatting
    public static class IndianCurrencyHelper
    {
        private static readonly CultureInfo IndianCulture = new CultureInfo("en-IN");

        // Format as Indian Rupees
        public static string FormatRupees(decimal amount)
        {
            return "₹" + amount.ToString("N2", IndianCulture);
        }

        // Format without symbol
        public static string FormatAmount(decimal amount)
        {
            return amount.ToString("N2", IndianCulture);
        }

        // Format with words (for invoices)
        public static string FormatAmountInWords(decimal amount)
        {
            // This is a simplified version
            // For production, use a proper number-to-words library
            return $"{FormatRupees(amount)} Only";
        }
    }
}


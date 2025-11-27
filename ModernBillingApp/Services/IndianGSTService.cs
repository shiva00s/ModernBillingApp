using ModernBillingApp.Models;
using System.Globalization;

namespace ModernBillingApp.Services
{
    // Service for Indian GST calculations and compliance
    public class IndianGSTService
    {
        // Calculate CGST/SGST for intra-state or IGST for inter-state
        public (decimal cgst, decimal sgst, decimal igst) CalculateGSTSplit(
            decimal amount, 
            decimal gstRate, 
            bool isInterState)
        {
            decimal totalGST = (amount * gstRate) / 100;
            
            if (isInterState)
            {
                // Inter-state: Full GST as IGST
                return (0, 0, totalGST);
            }
            else
            {
                // Intra-state: Split equally into CGST and SGST
                decimal halfGST = totalGST / 2;
                return (halfGST, halfGST, 0);
            }
        }

        // Validate GSTIN format (15 characters, alphanumeric)
        public bool ValidateGSTIN(string? gstin)
        {
            if (string.IsNullOrWhiteSpace(gstin))
                return false;
                
            // GSTIN format: 15 characters, alphanumeric
            if (gstin.Length != 15)
                return false;
                
            return System.Text.RegularExpressions.Regex.IsMatch(gstin, @"^[0-9A-Z]{15}$");
        }

        // Validate HSN Code (4, 6, or 8 digits)
        public bool ValidateHSNCode(string? hsn)
        {
            if (string.IsNullOrWhiteSpace(hsn))
                return false;
                
            // HSN can be 4, 6, or 8 digits
            return System.Text.RegularExpressions.Regex.IsMatch(hsn, @"^\d{4,8}$");
        }

        // Format Indian currency
        public string FormatIndianCurrency(decimal amount)
        {
            return "₹" + amount.ToString("N2", new CultureInfo("en-IN"));
        }

        // Get GST rate category name
        public string GetGSTRateCategory(decimal rate)
        {
            return rate switch
            {
                0 => "Exempt",
                5 => "5%",
                12 => "12%",
                18 => "18%",
                28 => "28%",
                _ => $"{rate}%"
            };
        }
    }
}


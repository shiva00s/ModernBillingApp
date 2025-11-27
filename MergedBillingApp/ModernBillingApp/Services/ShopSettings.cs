using System.ComponentModel.DataAnnotations;
using ModernBillingApp.Data;
using ModernBillingApp.Models;

// This is a new model to store your shop's details
public class ShopSettings
{
    public int Id { get; set; } // Will always be 1

    [Required]
    public string? ShopName { get; set; }
    public string? Address { get; set; }
    public string? ContactNumber { get; set; }
    public string? GstNumber { get; set; }
    public byte[]? Logo { get; set; } // We will store the logo image here
}
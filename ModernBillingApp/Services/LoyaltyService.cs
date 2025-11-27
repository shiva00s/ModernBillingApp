using Microsoft.EntityFrameworkCore;
using ModernBillingApp.Data;
using ModernBillingApp.Models;

namespace ModernBillingApp.Services
{
    // Loyalty Points Management Service
    public class LoyaltyService
    {
        private readonly AppDbContext _context;

        public LoyaltyService(AppDbContext context)
        {
            _context = context;
        }

        // Earn points on purchase
        public async Task EarnPoints(int customerId, int billId, decimal billAmount, decimal pointsPerRupee = 0.01m)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var customer = await _context.Customers.FindAsync(customerId);
                if (customer == null)
                    throw new InvalidOperationException("Customer not found");

                decimal pointsEarned = Math.Floor(billAmount * pointsPerRupee * 100) / 100; // Round to 2 decimals

                if (pointsEarned > 0)
                {
                    var transaction_ = new LoyaltyTransaction
                    {
                        CustomerId = customerId,
                        TransactionDate = DateTime.Now,
                        TransactionType = "Earn",
                        Points = pointsEarned,
                        BillId = billId,
                        BillNo = (await _context.Bills.FindAsync(billId))?.BillNo,
                        Description = $"Points earned on purchase",
                        BalanceAfter = customer.Points + pointsEarned,
                        ExpiryDate = DateTime.Now.AddYears(1) // Points expire after 1 year
                    };

                    _context.LoyaltyTransactions.Add(transaction_);

                    customer.Points += pointsEarned;
                    customer.TotalPointsEarned += pointsEarned;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Redeem points
        public async Task<bool> RedeemPoints(int customerId, decimal pointsToRedeem, string? description = null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var customer = await _context.Customers.FindAsync(customerId);
                if (customer == null)
                    throw new InvalidOperationException("Customer not found");

                if (customer.Points < pointsToRedeem)
                    throw new InvalidOperationException(
                        $"Insufficient points. Available: {customer.Points}, Required: {pointsToRedeem}");

                var transaction_ = new LoyaltyTransaction
                {
                    CustomerId = customerId,
                    TransactionDate = DateTime.Now,
                    TransactionType = "Redeem",
                    Points = -pointsToRedeem, // Negative for redemption
                    Description = description ?? "Points redeemed",
                    BalanceAfter = customer.Points - pointsToRedeem
                };

                _context.LoyaltyTransactions.Add(transaction_);

                customer.Points -= pointsToRedeem;
                customer.TotalPointsRedeemed += pointsToRedeem;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Get customer loyalty summary
        public async Task<LoyaltySummary> GetCustomerLoyaltySummary(int customerId)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
                throw new InvalidOperationException("Customer not found");

            var transactions = await _context.LoyaltyTransactions
                .Where(t => t.CustomerId == customerId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            var expiringSoon = transactions
                .Where(t => t.ExpiryDate.HasValue && 
                           t.ExpiryDate.Value > DateTime.Now && 
                           t.ExpiryDate.Value <= DateTime.Now.AddDays(30) &&
                           t.TransactionType == "Earn")
                .Sum(t => t.Points);

            return new LoyaltySummary
            {
                CustomerId = customerId,
                CurrentPoints = customer.Points,
                TotalEarned = customer.TotalPointsEarned,
                TotalRedeemed = customer.TotalPointsRedeemed,
                ExpiringSoon = expiringSoon,
                Transactions = transactions
            };
        }

        // Convert points to discount amount (e.g., 100 points = ₹1)
        public decimal PointsToAmount(decimal points, decimal conversionRate = 0.01m)
        {
            return points * conversionRate;
        }
    }

    public class LoyaltySummary
    {
        public int CustomerId { get; set; }
        public decimal CurrentPoints { get; set; }
        public decimal TotalEarned { get; set; }
        public decimal TotalRedeemed { get; set; }
        public decimal ExpiringSoon { get; set; }
        public List<LoyaltyTransaction> Transactions { get; set; } = new();
    }
}


using Microsoft.EntityFrameworkCore;
using ModernBillingApp.Data;
using ModernBillingApp.Models; 

// This service replaces all the query logic from frmCustomerEntry.cs
public class CustomerService
{
    private readonly AppDbContext _context;

    public CustomerService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Customer>> GetCustomers()
    {
        // Gets all customers, including their payment history
        return await _context.Customers.Include(c => c.Payments).ToListAsync();
    }

    public async Task<Customer> GetCustomerById(int id)
    {
        return await _context.Customers.Include(c => c.Payments)
                                    .FirstOrDefaultAsync(c => c.Id == id);
    }

    // This replaces your "Save" button (button6_Click)
    public async Task<Customer> CreateCustomer(Customer customer)
    {
        customer.CDate = DateTime.Now; // Set the creation date
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    // This replaces your "Update" button (button7_Click)
    public async Task UpdateCustomer(Customer customer)
    {
        _context.Entry(customer).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    // This replaces your "Delete" button (button4_Click)
    public async Task DeleteCustomer(int customerId)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        if (customer != null)
        {
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }
    }
}
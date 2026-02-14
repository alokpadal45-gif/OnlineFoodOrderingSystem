using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineFoodOrderingSystem.Data;
using OnlineFoodOrderingSystem.Models;

namespace OnlineFoodOrderingSystem.Controllers
{
    // Controller for managing customer information
    // Handles customer registration, profile viewing, editing, and deletion
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor - sets up database access
        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customer
        // Displays a list of all registered customers
        public async Task<IActionResult> Index()
        {
            // Get all customers from database
            var customers = await _context.Customers.ToListAsync();
            return View(customers);
        }

        // GET: Customer/Details/5
        // Shows detailed information about a specific customer
        // Including their order history
        public async Task<IActionResult> Details(int? id)
        {
            // Check if customer ID was provided
            if (id == null)
            {
                return NotFound();
            }

            // Get customer with their order history
            var customer = await _context.Customers
                .Include(c => c.Orders) // Load all orders placed by this customer
                .FirstOrDefaultAsync(m => m.CustomerId == id);

            // Return 404 if customer not found
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customer/Create
        // Displays registration form for a new customer
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customer/Create
        // Processes new customer registration
        [HttpPost]
        [ValidateAntiForgeryToken] // Prevents cross-site request forgery
        public async Task<IActionResult> Create([Bind("CustomerId,CustomerName,Email,PhoneNumber,Address")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists
                var existingCustomer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.Email == customer.Email);

                if (existingCustomer != null)
                {
                    // Email already registered
                    ModelState.AddModelError("Email", "This email address is already registered");
                    return View(customer);
                }

                // Save new customer to database
                _context.Add(customer);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Customer registered successfully!";
                
                return RedirectToAction(nameof(Index));
            }

            // If validation failed, show form with errors
            return View(customer);
        }

        // GET: Customer/Edit/5
        // Displays form to edit customer information
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Find customer to edit
            var customer = await _context.Customers.FindAsync(id);
            
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customer/Edit/5
        // Saves changes to customer information
        [HttpPost]
        [ValidateAntiForgeryToken] // Security protection
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,CustomerName,Email,PhoneNumber,Address")] Customer customer)
        {
            // Verify customer ID matches
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Check if email is being changed to one that already exists
                    var existingCustomer = await _context.Customers
                        .FirstOrDefaultAsync(c => c.Email == customer.Email && c.CustomerId != customer.CustomerId);

                    if (existingCustomer != null)
                    {
                        ModelState.AddModelError("Email", "This email address is already in use");
                        return View(customer);
                    }

                    // Update customer information
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Customer information updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Handle case where customer was deleted by another user
                    if (!CustomerExists(customer.CustomerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw; // Re-throw if different concurrency issue
                    }
                }
                
                return RedirectToAction(nameof(Index));
            }

            return View(customer);
        }

        // GET: Customer/Delete/5
        // Shows confirmation page before deleting a customer
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get customer including order count for warning
            var customer = await _context.Customers
                .Include(c => c.Orders) // Load orders to check if customer has order history
                .FirstOrDefaultAsync(m => m.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            // Warn if customer has existing orders
            if (customer.Orders != null && customer.Orders.Any())
            {
                ViewData["WarningMessage"] = $"This customer has {customer.Orders.Count} order(s). Deleting will remove all associated data.";
            }

            return View(customer);
        }

        // POST: Customer/Delete/5
        // Permanently deletes a customer and their order history
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken] // Security protection
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Find and delete the customer
            var customer = await _context.Customers.FindAsync(id);
            
            if (customer != null)
            {
                // Deleting customer will cascade delete their orders due to foreign key relationship
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Customer deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Customer/Search
        // Allows searching for customers by name, email, or phone
        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                // If no search term, show all customers
                return RedirectToAction(nameof(Index));
            }

            // Search across multiple fields
            var customers = await _context.Customers
                .Where(c => c.CustomerName.Contains(searchTerm) ||
                           c.Email.Contains(searchTerm) ||
                           c.PhoneNumber.Contains(searchTerm))
                .ToListAsync();

            // Pass search term to view for display
            ViewData["SearchTerm"] = searchTerm;
            
            return View("Index", customers);
        }

        // Helper method - checks if a customer exists
        // Used for validation and error checking
        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
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
            var customers = await _context.Customers.ToListAsync();
            return View(customers);
        }

        // GET: Customer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(m => m.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customer/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,CustomerName,Email,PhoneNumber,Address")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists
                var existingCustomer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.Email == customer.Email);

                if (existingCustomer != null)
                {
                    ModelState.AddModelError("Email", "This email address is already registered");
                    return View(customer);
                }

                _context.Add(customer);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Customer registered successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(customer);
        }

        // GET: Customer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,CustomerName,Email,PhoneNumber,Address")] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCustomer = await _context.Customers
                        .FirstOrDefaultAsync(c => c.Email == customer.Email && c.CustomerId != customer.CustomerId);

                    if (existingCustomer != null)
                    {
                        ModelState.AddModelError("Email", "This email address is already in use");
                        return View(customer);
                    }

                    _context.Update(customer);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Customer information updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(customer);
        }

        // GET: Customer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(m => m.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            if (customer.Orders != null && customer.Orders.Any())
            {
                ViewData["WarningMessage"] = $"This customer has {customer.Orders.Count} order(s). Deleting will remove all associated data.";
            }

            return View(customer);
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Load customer with all related orders and order details
            var customer = await _context.Customers
                .Include(c => c.Orders)
                    .ThenInclude(o => o.OrderDetails)
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer != null)
            {
                // Step 1: Delete all order details for each order
                foreach (var order in customer.Orders)
                {
                    _context.OrderDetails.RemoveRange(order.OrderDetails);
                }

                // Step 2: Delete all orders belonging to this customer
                _context.Orders.RemoveRange(customer.Orders);

                // Step 3: Delete the customer
                _context.Customers.Remove(customer);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Customer deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Customer/Search
        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return RedirectToAction(nameof(Index));
            }

            var customers = await _context.Customers
                .Where(c => c.CustomerName.Contains(searchTerm) ||
                           c.Email.Contains(searchTerm) ||
                           c.PhoneNumber.Contains(searchTerm))
                .ToListAsync();

            ViewData["SearchTerm"] = searchTerm;

            return View("Index", customers);
        }

        // Helper method - checks if a customer exists
        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
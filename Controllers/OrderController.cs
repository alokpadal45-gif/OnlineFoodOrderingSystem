using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineFoodOrderingSystem.Data;
using OnlineFoodOrderingSystem.Models;

namespace OnlineFoodOrderingSystem.Controllers
{
    // Controller responsible for managing customer orders
    // Handles order creation, viewing, editing, status updates, and deletion
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor - initializes database context for order operations
        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Order
        // Displays a list of all orders with customer information
        // Orders are sorted by date with newest first
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer) // Load customer details for each order
                .OrderByDescending(o => o.OrderDate) // Sort by newest orders first
                .ToListAsync();
            
            return View(orders);
        }

        // GET: Order/Details/5
        // Shows detailed information about a specific order
        // Includes customer info, ordered items, and food details
        public async Task<IActionResult> Details(int? id)
        {
            // Check if order ID was provided
            if (id == null)
            {
                return NotFound();
            }

            // Fetch order with all related data
            var order = await _context.Orders
                .Include(o => o.Customer) // Get customer who placed the order
                .Include(o => o.OrderDetails) // Get all items in this order
                    .ThenInclude(od => od.Food) // Get food details for each item
                .FirstOrDefaultAsync(m => m.OrderId == id);

            // Return 404 if order doesn't exist
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Order/Create
        // Displays the form to create a new order
        // Loads customer dropdown list for selection
        public IActionResult Create()
        {
            // Populate dropdown with all customers
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerName");
            return View();
        }

        // POST: Order/Create
        // Processes the new order form submission
        // Creates order and redirects to add order items
        [HttpPost]
        [ValidateAntiForgeryToken] // Security measure against cross-site request forgery
        public async Task<IActionResult> Create([Bind("OrderId,CustomerId,OrderDate,TotalAmount,OrderStatus,DeliveryAddress")] Order order)
        {
            if (ModelState.IsValid)
            {
                // Set order date to current time
                order.OrderDate = DateTime.Now;
                
                // Set default status if not provided
                order.OrderStatus = string.IsNullOrEmpty(order.OrderStatus) ? "Pending" : order.OrderStatus;
                
                // Save order to database
                _context.Add(order);
                await _context.SaveChangesAsync();
                
                // Show success message to user
                TempData["SuccessMessage"] = "Order created successfully!";
                
                // Redirect to add items to this order
                return RedirectToAction("Create", "OrderDetails", new { orderId = order.OrderId });
            }

            // If validation failed, reload the form with customer list
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerName", order.CustomerId);
            return View(order);
        }

        // GET: Order/Edit/5
        // Displays form to edit an existing order's basic information
        public async Task<IActionResult> Edit(int? id)
        {
            // Validate order ID
            if (id == null)
            {
                return NotFound();
            }

            // Find the order to edit
            var order = await _context.Orders.FindAsync(id);
            
            if (order == null)
            {
                return NotFound();
            }

            // Load customer dropdown with current customer pre-selected
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerName", order.CustomerId);
            return View(order);
        }

        // POST: Order/Edit/5
        // Saves changes to an existing order
        [HttpPost]
        [ValidateAntiForgeryToken] // Prevents cross-site request forgery attacks
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,CustomerId,OrderDate,TotalAmount,OrderStatus,DeliveryAddress")] Order order)
        {
            // Verify the order ID matches
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update order in database
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Order updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Handle case where order was deleted by another user
                    if (!OrderExists(order.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw; // Re-throw if it's a different concurrency issue
                    }
                }
                
                return RedirectToAction(nameof(Index));
            }

            // If validation failed, reload form
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerName", order.CustomerId);
            return View(order);
        }

        // GET: Order/Delete/5
        // Shows confirmation page before deleting an order
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Load order with customer information for display
            var order = await _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Order/Delete/5
        // Permanently deletes an order from the database
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken] // Security protection
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Find and delete the order
            var order = await _context.Orders.FindAsync(id);
            
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Order deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Order/UpdateStatus/5
        // Displays form to update order status (Pending, Confirmed, Delivered, etc.)
        public async Task<IActionResult> UpdateStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get order with customer details
            var order = await _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            // Define available order statuses
            var statuses = new List<string>
            {
                "Pending",       // Order received, awaiting confirmation
                "Confirmed",     // Order confirmed by restaurant
                "Preparing",     // Food is being prepared
                "Out for Delivery", // Order is on the way
                "Delivered",     // Order successfully delivered
                "Cancelled"      // Order was cancelled
            };

            // Create dropdown list with current status pre-selected
            ViewData["OrderStatuses"] = new SelectList(statuses, order.OrderStatus);
            
            return View(order);
        }

        // POST: Order/UpdateStatus/5
        // Updates the status of an order (e.g., from Pending to Confirmed)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string orderStatus)
        {
            // Find the order to update
            var order = await _context.Orders.FindAsync(id);
            
            if (order == null)
            {
                return NotFound();
            }

            // Update status and save changes
            order.OrderStatus = orderStatus;
            await _context.SaveChangesAsync();
            
            // Notify user of successful update
            TempData["SuccessMessage"] = $"Order status updated to '{orderStatus}'";
            
            // Return to order details page
            return RedirectToAction(nameof(Details), new { id = id });
        }

        // Helper method - checks if an order exists in the database
        // Used for validation and concurrency checks
        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
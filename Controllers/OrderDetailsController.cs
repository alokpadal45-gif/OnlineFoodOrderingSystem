using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineFoodOrderingSystem.Data;
using OnlineFoodOrderingSystem.Models;

namespace OnlineFoodOrderingSystem.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: OrderDetails/Create?orderId=5
        // Displays form to add items to an order
        public async Task<IActionResult> Create(int? orderId)
        {
            if (orderId == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            // Get all available foods
            ViewData["FoodId"] = new SelectList(_context.Foods, "FoodId", "Name");
            ViewData["OrderId"] = orderId;
            return View();
        }

        // POST: OrderDetails/Create
        // Adds an item to an order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int orderId, int foodId, int quantity)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            var food = await _context.Foods.FindAsync(foodId);
            if (food == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid && quantity > 0)
            {
                // FIXED: Changed OrderDetails to OrderDetail
                var orderDetail = new OrderDetail
                {
                    OrderId = orderId,
                    FoodId = foodId,
                    Quantity = quantity,
                    UnitPrice = food.Price,
                    TotalPrice = food.Price * quantity  // FIXED: Changed Subtotal to TotalPrice
                };

                _context.Add(orderDetail);
                
                // Update order total
                order.TotalAmount += orderDetail.TotalPrice;
                _context.Update(order);
                
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Item added to order!";
                return RedirectToAction("Create", new { orderId = orderId });
            }

            ViewData["FoodId"] = new SelectList(_context.Foods, "FoodId", "Name", foodId);
            ViewData["OrderId"] = orderId;
            return View();
        }

        // GET: OrderDetails/RemoveItem/5
        // Removes an item from an order
        public async Task<IActionResult> RemoveItem(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Food)
                .FirstOrDefaultAsync(m => m.OrderDetailId == id);

            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // POST: OrderDetails/RemoveItem/5
        [HttpPost, ActionName("RemoveItem")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItemConfirmed(int id)
        {
            var orderDetail = await _context.OrderDetails
                .Include(od => od.Order)
                .FirstOrDefaultAsync(m => m.OrderDetailId == id);

            if (orderDetail != null)
            {
                var orderId = orderDetail.OrderId;
                var order = orderDetail.Order;

                // FIXED: Added null check for order
                if (order != null)
                {
                    // Update order total - using Subtotal property which returns TotalPrice
                    order.TotalAmount -= orderDetail.Subtotal;
                    _context.Update(order);
                }

                _context.OrderDetails.Remove(orderDetail);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Item removed from order!";
                return RedirectToAction("Create", new { orderId = orderId });
            }

            return NotFound();
        }

        // GET: OrderDetails/AddItem?orderId=5
        // Shows form to add a specific item to an order
        public async Task<IActionResult> AddItem(int? orderId)
        {
            if (orderId == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            ViewData["FoodId"] = new SelectList(_context.Foods, "FoodId", "Name");
            ViewData["OrderId"] = orderId;
            return View();
        }

        // Check if order detail exists
        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetails.Any(e => e.OrderDetailId == id);
        }
    }
}
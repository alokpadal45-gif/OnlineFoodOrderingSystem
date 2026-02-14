using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineFoodOrderingSystem.Data;
using OnlineFoodOrderingSystem.Models;
using OnlineFoodOrderingSystem.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineFoodOrderingSystem.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Admin"))
            {
                return await AdminDashboard();
            }
            else if (roles.Contains("Manager"))
            {
                return await ManagerDashboard();
            }
            else if (roles.Contains("Staff"))
            {
                return await StaffDashboard();
            }
            else if (roles.Contains("Customer"))
            {
                return await CustomerDashboard();
            }

            return View("Index");
        }

        [Authorize(Roles = "Admin")]
        private async Task<IActionResult> AdminDashboard()
        {
            var viewModel = new DashboardViewModel
            {
                TotalUsers = await _context.Users.CountAsync(),

                TotalCustomers = await _context.Users
                    .Where(u => u.UserRoles.Any(ur => ur.Role.Name == "Customer"))
                    .CountAsync(),

                TotalFoodItems = await _context.Foods.CountAsync(),

                TotalOrders = await _context.Orders.CountAsync(),

                PendingOrders = await _context.Orders
                    .Where(o => o.Status != "Delivered")
                    .CountAsync(),

                CompletedOrders = await _context.Orders
                    .Where(o => o.Status == "Delivered")
                    .CountAsync(),

                // FIXED: Use AsEnumerable() to calculate sum on client side
                TotalRevenue = _context.Orders
                    .Where(o => o.Status == "Delivered")
                    .AsEnumerable()
                    .Sum(o => o.TotalAmount),

                RecentOrders = await _context.Orders
                    .Include(o => o.User)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(10)
                    .ToListAsync(),

                UserMenus = await GetUserMenus()
            };

            return View("AdminDashboard", viewModel);
        }

        [Authorize(Roles = "Manager")]
        private async Task<IActionResult> ManagerDashboard()
        {
            var viewModel = new DashboardViewModel
            {
                TotalOrders = await _context.Orders.CountAsync(),
                
                PendingOrders = await _context.Orders
                    .Where(o => o.Status != "Delivered")
                    .CountAsync(),
                
                CompletedOrders = await _context.Orders
                    .Where(o => o.Status == "Delivered")
                    .CountAsync(),
                
                // FIXED: Use AsEnumerable()
                TotalRevenue = _context.Orders
                    .Where(o => o.Status == "Delivered")
                    .AsEnumerable()
                    .Sum(o => o.TotalAmount),
                
                RecentOrders = await _context.Orders
                    .Include(o => o.User)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(10)
                    .ToListAsync(),
                
                UserMenus = await GetUserMenus()
            };

            return View("ManagerDashboard", viewModel);
        }

        [Authorize(Roles = "Staff")]
        private async Task<IActionResult> StaffDashboard()
        {
            var viewModel = new DashboardViewModel
            {
                PendingOrders = await _context.Orders
                    .Where(o => o.Status != "Delivered")
                    .CountAsync(),
                
                RecentOrders = await _context.Orders
                    .Include(o => o.User)
                    .Where(o => o.Status != "Delivered")
                    .OrderByDescending(o => o.OrderDate)
                    .Take(10)
                    .ToListAsync(),
                
                UserMenus = await GetUserMenus()
            };

            return View("StaffDashboard", viewModel);
        }

        [Authorize(Roles = "Customer")]
        private async Task<IActionResult> CustomerDashboard()
        {
            var user = await _userManager.GetUserAsync(User);

            var viewModel = new DashboardViewModel
            {
                TotalOrders = await _context.Orders
                    .Where(o => o.UserId == user.Id)
                    .CountAsync(),

                PendingOrders = await _context.Orders
                    .Where(o => o.UserId == user.Id && o.Status != "Delivered")
                    .CountAsync(),

                RecentOrders = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Food)
                    .Where(o => o.UserId == user.Id)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(10)
                    .ToListAsync(),

                UserMenus = await GetUserMenus()
            };

            return View("CustomerDashboard", viewModel);
        }

        private async Task<List<Menu>> GetUserMenus()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return new List<Menu>();
            }

            var menus = await _context.UserMenus
                .Where(um => um.UserId == user.Id && um.IsActive)
                .Include(um => um.Menu)
                .Where(um => um.Menu.IsActive)
                .OrderBy(um => um.Menu.DisplayOrder)
                .Select(um => um.Menu)
                .ToListAsync();

            return menus;
        }
    }
}
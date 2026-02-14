using OnlineFoodOrderingSystem.Models;
using System.Collections.Generic;

namespace OnlineFoodOrderingSystem.ViewModels
{
    // View model for dashboard views containing statistics and data for different user roles
    // This model holds all the data that will be displayed on the dashboard
    public class DashboardViewModel
    {
        // User Statistics

        // Total number of users in the system
        public int TotalUsers { get; set; }

        // Total number of customers (users with Customer role)
        public int TotalCustomers { get; set; }

        // Food Statistics

        // Total number of food items available
        public int TotalFoodItems { get; set; }

        // Order Statistics

        // Total number of orders placed
        public int TotalOrders { get; set; }

        // Number of orders that are pending/in-progress
        public int PendingOrders { get; set; }

        // Number of completed/delivered orders
        public int CompletedOrders { get; set; }

        // Financial Statistics

        // Total revenue from completed orders
        public decimal TotalRevenue { get; set; }

        // Data Collections

        // List of recent orders to display on dashboard
        public List<Order> RecentOrders { get; set; } = new List<Order>();

        // List of menus accessible to the current user
        // Used for rendering the navigation menu
        public List<Menu> UserMenus { get; set; } = new List<Menu>();
    }
}
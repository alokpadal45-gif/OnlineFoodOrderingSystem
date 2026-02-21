using Microsoft.AspNetCore.Identity;
using OnlineFoodOrderingSystem.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineFoodOrderingSystem.Data
{
    // Class responsible for seeding initial data into the database.
    // This includes creating default roles, admin user, and menu items.
    // Seeding is important to have the system ready to use with default data
    public static class DbSeeder
    {
        // Seeds the database with initial data including roles, admin user, and menus.
        // This method should be called during application startup in Program.cs
        public static async Task SeedDatabase(ApplicationDbContext context, 
            UserManager<User> userManager, 
            RoleManager<Role> roleManager)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Seed Roles - create the 4 default roles
            await SeedRoles(roleManager);

            // Seed Admin User - create the default admin account
            await SeedAdminUser(userManager);

            // Seed Menus - create the default navigation menu items
            await SeedMenus(context);

            // Assign Menus to Admin - give admin access to all menus
            await AssignMenusToAdmin(context, userManager);
        }

        // Creates default roles in the system: Admin, Customer, Staff, Manager
        // Each role has different permissions and access levels
        private static async Task SeedRoles(RoleManager<Role> roleManager)
        {
            // Define default roles with descriptions
            string[] roleNames = { "Admin", "Customer", "Staff", "Manager" };
            string[] roleDescriptions = {
                "Full system access with all administrative privileges",
                "Regular customer who can browse and order food",
                "Restaurant staff who can manage orders and food items",
                "Manager who can oversee operations and generate reports"
            };

            // Create each role if it doesn't exist
            for (int i = 0; i < roleNames.Length; i++)
            {
                // Check if role already exists to avoid duplicates
                if (!await roleManager.RoleExistsAsync(roleNames[i]))
                {
                    // Create new role with description
                    var role = new Role
                    {
                        Name = roleNames[i],
                        Description = roleDescriptions[i],
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    };

                    // Add role to database using RoleManager
                    await roleManager.CreateAsync(role);
                }
            }
        }

        // Creates the default admin user if it doesn't exist
        // Admin user has full access to the system
        private static async Task SeedAdminUser(UserManager<User> userManager)
        {
            // Define admin credentials - CHANGE THESE IN PRODUCTION!
            string adminEmail = "admin@foodsystem.com";
            string adminPassword = "Admin@123";

            // Check if admin user already exists
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                // Create new admin user with all required fields
                adminUser = new User
                {
                    UserName = adminEmail,           // We use email as username
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Administrator",
                    PhoneNumber = "1234567890",
                    Address = "System Address",
                    EmailConfirmed = true,           // Auto-confirm email for admin
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                // Create user with password using UserManager
                // UserManager automatically hashes the password for security
                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    // Assign Admin role to the user
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

        // Creates default menu items in the system
        // These are the navigation items that users will see based on their permissions
        private static async Task SeedMenus(ApplicationDbContext context)
        {
            // Check if menus already exist to avoid duplicates
            if (context.Menus.Any())
            {
                return; // Menus already seeded, exit method
            }

            // Create menu items with all required properties
            var menus = new[]
            {
                new Menu
                {
                    MenuName = "Dashboard",
                    MenuUrl = "/Dashboard/Index",
                    IconClass = "fas fa-tachometer-alt",
                    DisplayOrder = 1,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new Menu
                {
                    MenuName = "Food Management",
                    MenuUrl = "/Food/Index",
                    IconClass = "fas fa-utensils",
                    DisplayOrder = 2,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new Menu
                {
                    MenuName = "Order Management",
                    MenuUrl = "/Order/Index",
                    IconClass = "fas fa-shopping-cart",
                    DisplayOrder = 3,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new Menu
                {
                    MenuName = "Customer Management",
                    MenuUrl = "/Customer/Index",
                    IconClass = "fas fa-users",
                    DisplayOrder = 4,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new Menu
                {
                    MenuName = "User Management",
                    MenuUrl = "/User/Index",
                    IconClass = "fas fa-user-cog",
                    DisplayOrder = 5,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new Menu
                {
                    MenuName = "Role Management",
                    MenuUrl = "/Role/Index",
                    IconClass = "fas fa-user-shield",
                    DisplayOrder = 6,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new Menu
                {
                    MenuName = "Reports",
                    MenuUrl = "/Report/Index",
                    IconClass = "fas fa-chart-bar",
                    DisplayOrder = 7,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                }
            };

            // Add menus to database
            await context.Menus.AddRangeAsync(menus);
            await context.SaveChangesAsync();
        }

        // Assigns all menus to the admin user
        // This ensures admin has access to all parts of the system
        private static async Task AssignMenusToAdmin(ApplicationDbContext context, UserManager<User> userManager)
        {
            // Find admin user
            var adminUser = await userManager.FindByEmailAsync("admin@foodsystem.com");

            if (adminUser == null)
            {
                return; // Admin user not found, exit method
            }

            // Check if admin already has menu assignments to avoid duplicates
            if (context.UserMenus.Any(um => um.UserId == adminUser.Id))
            {
                return; // Menus already assigned, exit method
            }

            // Get all menus from database
            var allMenus = context.Menus.ToList();

            // Assign all menus to admin by creating UserMenu records
            foreach (var menu in allMenus)
            {
                var userMenu = new UserMenu
                {
                    UserId = adminUser.Id,
                    MenuId = menu.MenuId,
                    AssignedDate = DateTime.Now,
                    AssignedBy = "System",
                    IsActive = true
                };

                context.UserMenus.Add(userMenu);
            }

            // Save all changes to database
            await context.SaveChangesAsync();
        }
    }
}
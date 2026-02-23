using Microsoft.AspNetCore.Identity;
using OnlineFoodOrderingSystem.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineFoodOrderingSystem.Data
{
    public static class DbSeeder
    {
        public static async Task SeedDatabase(ApplicationDbContext context,
            UserManager<User> userManager,
            RoleManager<Role> roleManager)
        {
            // Seed Roles - create the 4 default roles
            await SeedRoles(roleManager);

            // Seed Admin User
            await SeedAdminUser(userManager);

            // Seed Alok as Admin  ← NEW
            await SeedAlokAsAdmin(userManager);

            // Seed Menus
            await SeedMenus(context);

            // Assign Menus to Admin
            await AssignMenusToAdmin(context, userManager);
        }

        private static async Task SeedRoles(RoleManager<Role> roleManager)
        {
            string[] roleNames = { "Admin", "Customer", "Staff", "Manager" };
            string[] roleDescriptions = {
                "Full system access with all administrative privileges",
                "Regular customer who can browse and order food",
                "Restaurant staff who can manage orders and food items",
                "Manager who can oversee operations and generate reports"
            };

            for (int i = 0; i < roleNames.Length; i++)
            {
                if (!await roleManager.RoleExistsAsync(roleNames[i]))
                {
                    var role = new Role
                    {
                        Name = roleNames[i],
                        Description = roleDescriptions[i],
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    };
                    await roleManager.CreateAsync(role);
                }
            }
        }

        private static async Task SeedAdminUser(UserManager<User> userManager)
        {
            string adminEmail = "admin@foodsystem.com";
            string adminPassword = "Admin@123";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Administrator",
                    PhoneNumber = "1234567890",
                    Address = "System Address",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

        // NEW METHOD - Seeds Alok's account as Admin
        private static async Task SeedAlokAsAdmin(UserManager<User> userManager)
        {
            string email = "alokpadal452@gmail.com";
            string password = "Hitman4545@";

            var existingUser = await userManager.FindByEmailAsync(email);

            if (existingUser == null)
            {
                // Create account if it doesn't exist
                var user = new User
                {
                    UserName = email,
                    Email = email,
                    FirstName = "Alok",
                    LastName = "Padal",
                    PhoneNumber = "9841122119",
                    Address = "Kathmandu",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
            else
            {
                // Account already exists, upgrade to Admin
                var roles = await userManager.GetRolesAsync(existingUser);
                await userManager.RemoveFromRolesAsync(existingUser, roles);
                await userManager.AddToRoleAsync(existingUser, "Admin");
            }
        }

        private static async Task SeedMenus(ApplicationDbContext context)
        {
            if (context.Menus.Any())
            {
                return;
            }

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

            await context.Menus.AddRangeAsync(menus);
            await context.SaveChangesAsync();
        }

        private static async Task AssignMenusToAdmin(ApplicationDbContext context, UserManager<User> userManager)
        {
            var adminUser = await userManager.FindByEmailAsync("admin@foodsystem.com");

            if (adminUser == null)
            {
                return;
            }

            if (context.UserMenus.Any(um => um.UserId == adminUser.Id))
            {
                return;
            }

            var allMenus = context.Menus.ToList();

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

            await context.SaveChangesAsync();
        }
    }
}
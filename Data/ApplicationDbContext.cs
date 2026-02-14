using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineFoodOrderingSystem.Models;

namespace OnlineFoodOrderingSystem.Data
{
    // Database context for the Online Food Ordering System.
    // Inherits from IdentityDbContext to support ASP.NET Core Identity with custom User and Role models.
    // This class manages all database operations and entity configurations.
    public class ApplicationDbContext : IdentityDbContext<User, Role, string, IdentityUserClaim<string>,
        UserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        // Constructor that receives database configuration options
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets represent tables in the database - each DbSet corresponds to one table

        // Customers table - represents customer information
        // Note: This is separate from User table for additional customer-specific data
        public DbSet<Customer> Customers { get; set; }

        // Foods table - represents food items available for ordering
        public DbSet<Food> Foods { get; set; }

        // Orders table - represents customer orders
        public DbSet<Order> Orders { get; set; }

        // OrderDetails table - represents individual items within an order
        public DbSet<OrderDetail> OrderDetails { get; set; }

        // Menus table - represents navigation menu items in the application
        public DbSet<Menu> Menus { get; set; }

        // UserMenus table - junction table mapping users to accessible menus
        public DbSet<UserMenu> UserMenus { get; set; }

        // Configures entity relationships and database constraints using Fluent API.
        // This method is called when the model is being created.
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Call base configuration to set up Identity tables
            base.OnModelCreating(builder);

            // Configure User-UserRole-Role relationship (Many-to-Many)
            // This configuration establishes that:
            // - A User can have multiple Roles (e.g., John can be both Admin and Manager)
            // - A Role can be assigned to multiple Users (e.g., Admin role can be assigned to John, Jane, etc.)
            builder.Entity<UserRole>(userRole =>
            {
                // Define composite primary key using UserId and RoleId
                // This means the combination of UserId and RoleId must be unique
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                // Configure relationship: UserRole → User (Many-to-One)
                userRole.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)           // A User has many UserRoles
                    .HasForeignKey(ur => ur.UserId)       // Foreign key is UserId
                    .IsRequired()                          // UserId is required
                    .OnDelete(DeleteBehavior.Cascade);    // Delete UserRoles when User is deleted

                // Configure relationship: UserRole → Role (Many-to-One)
                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)           // A Role has many UserRoles
                    .HasForeignKey(ur => ur.RoleId)       // Foreign key is RoleId
                    .IsRequired()                          // RoleId is required
                    .OnDelete(DeleteBehavior.Cascade);    // Delete UserRoles when Role is deleted
            });

            // Configure User-UserMenu-Menu relationship (Many-to-Many)
            // This configuration establishes that:
            // - A User can access multiple Menus (e.g., John can access Dashboard, Orders, Reports)
            // - A Menu can be accessible to multiple Users (e.g., Dashboard can be accessed by John, Jane, etc.)
            builder.Entity<UserMenu>(userMenu =>
            {
                // Define primary key
                userMenu.HasKey(um => um.UserMenuId);

                // Configure relationship: UserMenu → User (Many-to-One)
                userMenu.HasOne(um => um.User)
                    .WithMany(u => u.UserMenus)           // A User has many UserMenus
                    .HasForeignKey(um => um.UserId)       // Foreign key is UserId
                    .IsRequired()                          // UserId is required
                    .OnDelete(DeleteBehavior.Cascade);    // Delete UserMenus when User is deleted

                // Configure relationship: UserMenu → Menu (Many-to-One)
                userMenu.HasOne(um => um.Menu)
                    .WithMany(m => m.UserMenus)           // A Menu has many UserMenus
                    .HasForeignKey(um => um.MenuId)       // Foreign key is MenuId
                    .IsRequired()                          // MenuId is required
                    .OnDelete(DeleteBehavior.Cascade);    // Delete UserMenus when Menu is deleted

                // Create unique constraint to prevent duplicate user-menu assignments
                // This ensures a user can't be assigned the same menu twice
                userMenu.HasIndex(um => new { um.UserId, um.MenuId })
                    .IsUnique()
                    .HasDatabaseName("IX_UserMenu_UserId_MenuId");
            });

            // Configure Menu self-referencing relationship for hierarchical structure
            // This allows menus to have parent-child relationships (e.g., "Settings" parent with "Profile", "Security" children)
            builder.Entity<Menu>(menu =>
            {
                // Configure relationship: Menu → ParentMenu (Many-to-One, Optional)
                menu.HasOne(m => m.ParentMenu)
                    .WithMany(m => m.ChildMenus)          // A Menu can have many ChildMenus
                    .HasForeignKey(m => m.ParentMenuId)   // Foreign key is ParentMenuId
                    .OnDelete(DeleteBehavior.Restrict);   // Prevent deletion if child menus exist
            });

            // Configure User-Order relationship (One-to-Many)
            // One user can have many orders
            builder.Entity<Order>(order =>
            {
                order.HasOne(o => o.User)
                    .WithMany(u => u.Orders)              // A User can have many Orders
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Restrict);   // Prevent user deletion if they have orders
            });

            // Configure precision for decimal fields in Food table
            // This sets the decimal to have 18 total digits, with 2 after the decimal point
            builder.Entity<Food>()
                .Property(f => f.Price)
                .HasPrecision(18, 2);  // Example: 9999999999999999.99

            // Configure precision for decimal fields in OrderDetail table
            builder.Entity<OrderDetail>()
                .Property(od => od.UnitPrice)
                .HasPrecision(18, 2);

            builder.Entity<OrderDetail>()
                .Property(od => od.TotalPrice)
                .HasPrecision(18, 2);

            // Configure precision for decimal fields in Order table
            builder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);
        }
    }
}
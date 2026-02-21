using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineFoodOrderingSystem.Models
{
    // Represents a menu item/page in the navigation system.
    // Menus control which sections of the application users can access.
    // Example: Dashboard menu, Food Management menu, Order Management menu, etc.
    public class Menu
    {
        // Unique identifier for the menu item
        // Primary key for the Menu table
        [Key]
        public int MenuId { get; set; }

        // Display name of the menu item shown in navigation
        // Example: "Dashboard", "Orders", "Food Items", "Users"
        [Required]
        [StringLength(100)]
        public string MenuName { get; set; }

        // URL path or route to navigate when menu is clicked
        // Example: "/Dashboard/Index", "/Order/List", "/Food/Manage"
        [Required]
        [StringLength(200)]
        public string MenuUrl { get; set; }

        // CSS icon class to display alongside menu name
        // Example: "fa fa-dashboard", "bi bi-cart", "fas fa-utensils"
        [StringLength(50)]
        public string IconClass { get; set; }

        // Order in which menu items should appear in navigation
        // Lower numbers appear first (e.g., 1, 2, 3...)
        public int DisplayOrder { get; set; }

        // ID of parent menu for creating hierarchical/nested menus
        // Null for top-level menus, set to parent MenuId for sub-menus
        public int? ParentMenuId { get; set; }

        // Flag to indicate if this menu item is currently active/visible
        // Inactive menus are hidden from all users
        public bool IsActive { get; set; } = true;

        // Date and time when this menu was created
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties

        // Collection of user-menu mappings showing which users can access this menu
        // This enables the many-to-many relationship between Users and Menus
        public virtual ICollection<UserMenu> UserMenus { get; set; } = new List<UserMenu>();

        // Reference to parent menu for hierarchical menu structure
        // Used to create dropdown/nested menus
        [ForeignKey("ParentMenuId")]
        public virtual Menu ParentMenu { get; set; }

        // Collection of child menus under this menu
        // Used for creating multi-level navigation structures
        public virtual ICollection<Menu> ChildMenus { get; set; } = new List<Menu>();
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineFoodOrderingSystem.Models
{
    // Junction/Mapping table that creates many-to-many relationship between Users and Menus.
    // This table answers the question: "Which Menu is available to which User?"
    // Controls navigation access based on user permissions.
    // Example: If John can access Dashboard, there will be a record here with John's UserId and Dashboard's MenuId
    public class UserMenu
    {
        // Unique identifier for this user-menu mapping
        // Primary key for the UserMenu table
        [Key]
        public int UserMenuId { get; set; }

        // Foreign key referencing the User who has access to the menu
        // Links to User.Id
        [Required]
        public string UserId { get; set; }

        // Foreign key referencing the Menu that the user can access
        // Links to Menu.MenuId
        [Required]
        public int MenuId { get; set; }

        // Date and time when this menu access was granted to the user
        // Important for tracking permission changes
        public DateTime AssignedDate { get; set; } = DateTime.Now;

        // ID of the user/admin who granted this menu access
        // Useful for audit trails and accountability
        public string AssignedBy { get; set; }

        // Flag to indicate if this menu access is currently active
        // Allows temporary disabling without deleting the assignment
        public bool IsActive { get; set; } = true;

        // Navigation Properties

        // Reference to the User who has access to this menu
        // Enables navigation from UserMenu to User entity
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        // Reference to the Menu that the user can access
        // Enables navigation from UserMenu to Menu entity
        [ForeignKey("MenuId")]
        public virtual Menu Menu { get; set; }
    }
}
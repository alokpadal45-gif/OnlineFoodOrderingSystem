using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace OnlineFoodOrderingSystem.Models
{
    // Represents a user in the system. Inherits from IdentityUser to get built-in authentication features.
    // This class extends the default Identity user with custom properties like FirstName, LastName, Address, etc.
    public class User : IdentityUser
    {
        // User's first name - required field with max length of 50 characters
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        // User's last name - required field with max length of 50 characters
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        // User's full address for delivery purposes
        [StringLength(200)]
        public string Address { get; set; }

        // NOTE: PhoneNumber is NOT defined here because it already exists in IdentityUser base class
        // We inherit PhoneNumber from IdentityUser, so we don't redeclare it

        // Date and time when the user account was created
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Flag to indicate if the user account is active or disabled
        public bool IsActive { get; set; } = true;

        // Navigation Properties - these enable Entity Framework to establish relationships between tables

        // Collection of roles assigned to this user through UserRole junction table
        // This enables many-to-many relationship between Users and Roles
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        // Collection of menus accessible to this user through UserMenu junction table
        // This enables many-to-many relationship between Users and Menus
        public virtual ICollection<UserMenu> UserMenus { get; set; } = new List<UserMenu>();

        // Collection of orders placed by this user
        // One user can have many orders (One-to-Many relationship)
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
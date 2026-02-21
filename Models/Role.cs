using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace OnlineFoodOrderingSystem.Models
{
    // Represents a role in the system (e.g., Admin, Customer, Staff, Manager).
    // Inherits from IdentityRole to integrate with ASP.NET Core Identity framework.
    public class Role : IdentityRole
    {
        // Detailed description of what this role can do in the system
        // Helps administrators understand the purpose and permissions of each role
        [StringLength(500)]
        public string Description { get; set; }

        // Date and time when this role was created in the system
        // Useful for audit trails and role management
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Flag to indicate if this role is currently active in the system
        // Inactive roles cannot be assigned to new users
        public bool IsActive { get; set; } = true;

        // Navigation Properties

        // Collection of user-role mappings showing which users have this role
        // This enables the many-to-many relationship between Users and Roles
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
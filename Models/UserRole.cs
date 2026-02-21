using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace OnlineFoodOrderingSystem.Models
{
    // Junction/Mapping table that creates many-to-many relationship between Users and Roles.
    // This table answers the question: "Which User has what Role?"
    // Inherits from IdentityUserRole to integrate with ASP.NET Core Identity.
    // Example: If John has Admin role, there will be a record here with John's UserId and Admin's RoleId
    public class UserRole : IdentityUserRole<string>
    {
        // Date and time when this role was assigned to the user
        // Important for tracking when permissions were granted
        public DateTime AssignedDate { get; set; } = DateTime.Now;

        // ID of the user who assigned this role to the user
        // Useful for audit trails and accountability
        public string AssignedBy { get; set; }

        // Flag to indicate if this role assignment is currently active
        // Allows temporary disabling of roles without deleting the assignment
        public bool IsActive { get; set; } = true;

        // Navigation Properties

        // Reference to the User who has been assigned this role
        // UserId comes from the inherited IdentityUserRole class
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        // Reference to the Role that has been assigned to the user
        // RoleId comes from the inherited IdentityUserRole class
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
    }
}
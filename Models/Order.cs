using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineFoodOrderingSystem.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        // Add CustomerId for compatibility with old controllers
        public int? CustomerId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        // CHANGE THIS - Make OrderStatus both get AND set
        [NotMapped]  // Don't store in database, just maps to Status
        public string OrderStatus 
        { 
            get => Status; 
            set => Status = value; 
        }

        [StringLength(500)]
        public string? DeliveryAddress { get; set; }

        [StringLength(15)]
        public string? ContactNumber { get; set; }

        public DateTime? DeliveredDate { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        // Add Customer navigation for compatibility
        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Models
{
    public partial class Product
    {
        public Product()
        {
            Comments = new HashSet<Comment>();
            OrderItems = new HashSet<OrderItem>();
            Ratings = new HashSet<Rating>();
        }

        public int Id { get; set; }
        [Required]
        public string ProductName { get; set; } = null!;
        public int? SupplierId { get; set; }
        [Required]
        public int? CategoryId { get; set; }
        public int ViewCount { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public int Quantity { get; set; }
        public int SoldQuantity { get; set; }
        public decimal? Discount { get; set; }
        [Required]
        public string? Images { get; set; }
        [Required]
        public string? Description { get; set; }

        public virtual Category? Category { get; set; }
        public virtual Supplier? Supplier { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
    }
}

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
        public string ProductName { get; set; } = null!;
        public int? SupplierId { get; set; }
        public int? CategoryId { get; set; }
        public int ViewCount { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public int SoldQuantity { get; set; }
        public decimal? Discount { get; set; }
        public string? Images { get; set; }
        public string? Description { get; set; }

        public virtual Category? Category { get; set; }
        public virtual Supplier? Supplier { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
    }
}

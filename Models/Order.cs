using System;
using System.Collections.Generic;

namespace Store.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? VoucherId { get; set; }
        public int? PaidMethodId { get; set; }
        public bool Paid { get; set; }

        public virtual PaymentMethod? PaidMethod { get; set; }
        public virtual User User { get; set; } = null!;
        public virtual Voucher? Voucher { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}

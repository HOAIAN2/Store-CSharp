using System;
using System.Collections.Generic;

namespace Store.Models
{
    public partial class Voucher
    {
        public Voucher()
        {
            Orders = new HashSet<Order>();
        }

        public string Id { get; set; } = null!;
        public string VoucherName { get; set; } = null!;
        public decimal VoucherDiscount { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}

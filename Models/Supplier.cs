using System;
using System.Collections.Generic;

namespace Store.Models
{
    public partial class Supplier
    {
        public Supplier()
        {
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string SupplierName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;

namespace Store.Models
{
    public partial class PaymentMethod
    {
        public PaymentMethod()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}

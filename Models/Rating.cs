﻿using System;
using System.Collections.Generic;

namespace Store.Models
{
    public partial class Rating
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int? Rate { get; set; }

        public virtual Product Product { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}

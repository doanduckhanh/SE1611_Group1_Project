using System;
using System.Collections.Generic;

namespace SE1611_Group1_Project.Models
{
    public partial class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int? OrderId { get; set; }
        public int? FoodId { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }

        public virtual Food? Food { get; set; }
        public virtual Order? Order { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace SE1611_Group1_Project.Models
{
    public partial class Food
    {
        public Food()
        {
            Carts = new HashSet<Cart>();
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int FoodId { get; set; }
        public string? FoodName { get; set; }
        public decimal? FoodPrice { get; set; }
        public string? FoodImage { get; set; }
        public int? FoodStatus { get; set; }
        public int? CategoryId { get; set; }

        public virtual Category? Category { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}

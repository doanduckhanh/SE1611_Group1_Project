using System;
using System.Collections.Generic;

namespace SE1611_Group1_Project.Models
{
    public partial class Cart
    {
        public int RecordId { get; set; }
        public string? CartId { get; set; }
        public int? FoodId { get; set; }
        public int? Count { get; set; }
        public DateTime? DateCreated { get; set; }

        public virtual User? CartNavigation { get; set; }
        public virtual Food? Food { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace SE1611_Group1_Project.Models
{
    public partial class Category
    {
        public Category()
        {
            Foods = new HashSet<Food>();
        }

        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryDescription { get; set; }

        public virtual ICollection<Food> Foods { get; set; }
    }
}

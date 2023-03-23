using System;
using System.Collections.Generic;

namespace SE1611_Group1_Project.Models
{
    public partial class Promo
    {
        public string PromoCode { get; set; } = null!;
        public string? PromoValue { get; set; }
        public string? PromoDescribe { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SE1611_Group1_Project.Models;

namespace SE1611_Group1_Project.PromoManager
{
    public class DetailsModel : PageModel
    {
        private readonly SE1611_Group1_Project.Models.FoodOrderContext _context;

        public DetailsModel(SE1611_Group1_Project.Models.FoodOrderContext context)
        {
            _context = context;
        }

      public Promo Promo { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.Promos == null)
            {
                return NotFound();
            }

            var promo = await _context.Promos.FirstOrDefaultAsync(m => m.PromoCode == id);
            if (promo == null)
            {
                return NotFound();
            }
            else 
            {
                Promo = promo;
            }
            return Page();
        }
    }
}
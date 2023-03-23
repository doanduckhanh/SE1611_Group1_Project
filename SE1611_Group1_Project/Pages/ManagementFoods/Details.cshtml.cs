using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SE1611_Group1_Project.Models;

namespace SE1611_Group1_Project.Pages.ManagementFoods
{
    public class DetailsModel : PageModel
    {
        private readonly SE1611_Group1_Project.Models.FoodOrderContext _context;

        public DetailsModel(SE1611_Group1_Project.Models.FoodOrderContext context)
        {
            _context = context;
        }

      public Food Food { get; set; } = default!;
        public List<Category> listCategories { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            listCategories = _context.Categories.ToList();
            if (id == null || _context.Foods == null)
            {
                return NotFound();
            }

            var food = await _context.Foods.FirstOrDefaultAsync(m => m.FoodId == id);
            if (food == null)
            {
                return NotFound();
            }
            else 
            {
                Food = food;
            }
            return Page();
        }
    }
}

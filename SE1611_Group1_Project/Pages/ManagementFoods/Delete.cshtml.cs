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
    public class DeleteModel : PageModel
    {
        private readonly SE1611_Group1_Project.Models.FoodOrderContext _context;

        public DeleteModel(SE1611_Group1_Project.Models.FoodOrderContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Food Food { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Foods == null)
            {
                return NotFound();
            }
            var food = await _context.Foods.FindAsync(id);

            if (food != null)
            {
                Food = food;
                var carts = _context.Carts.Where(x => x.FoodId == food.FoodId);
                _context.Carts.RemoveRange(carts);
                var orderDetails = _context.OrderDetails.Where(x => x.FoodId == food.FoodId);
                _context.OrderDetails.RemoveRange(orderDetails);
                _context.Foods.Remove(Food);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("./Index");
        }
    }
}

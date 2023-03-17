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
    public class IndexModel : PageModel
    {
        private readonly SE1611_Group1_Project.Models.FoodOrderContext _context;

        public IndexModel(SE1611_Group1_Project.Models.FoodOrderContext context)
        {
            _context = context;
        }

        public IList<Food> Food { get;set; } = default!;
        public IList<Category> Category { get; set; } = default;

        public async Task OnGetAsync()
        {
            if (_context.Foods != null && _context.Categories != null)
            {
                Food = await _context.Foods
                .Include(f => f.Category).ToListAsync();
                Category = await _context.Categories.ToListAsync();
            }
        }
    }
}

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
        public string searchInput { get; set; } = string.Empty;
        [BindProperty]
        public int  categoryChoose { get; set; } = default!;

        public IndexModel(SE1611_Group1_Project.Models.FoodOrderContext context)
        {
            _context = context;
        }

        public IList<Food> Food { get;set; } = default!;
        public IList<Category> Category { get; set; } = default;
        public int sort { get; set; } = 0;

        public async Task OnGetAsync(string searchInput,int categoryChoose,int sort)
        {
            if (_context.Foods != null && _context.Categories != null)
            {
                Food = await _context.Foods
                .Include(f => f.Category).ToListAsync();
                Category = await _context.Categories.ToListAsync();
                if (!string.IsNullOrEmpty(searchInput))
                {
                    Food = Food.Where(food => food.FoodName.Contains(searchInput)).ToList();
                }
                var cc = categoryChoose;
                if (cc != 0)
                {
                    Food = Food.Where(f => f.CategoryId == cc).ToList();
                }
                if (sort == 0)
                {
                    sort = 1;
                    Food = Food.OrderBy(p => p.FoodPrice).ToList();

                        Food = Food.OrderByDescending(p => p.FoodPrice).ToList();
                    
                }
            }

        }
        public void SortByPrice()
        {
            Food = Food.OrderBy(p => p.FoodPrice).ToList();
        }

    }
}

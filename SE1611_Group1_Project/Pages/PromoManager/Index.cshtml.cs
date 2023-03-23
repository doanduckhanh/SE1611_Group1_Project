using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SE1611_Group1_Project.Models;

namespace SE1611_Group1_Project.PromoManager
{
    public class IndexModel : PageModel
    {
        private readonly SE1611_Group1_Project.Models.FoodOrderContext _context;

        public IndexModel(SE1611_Group1_Project.Models.FoodOrderContext context)
        {
            _context = context;
        }

        public IList<Promo> Promo { get;set; } = default!;

        public async Task OnGetAsync()
        {
            ViewData["Role"] = HttpContext.Session.GetInt32("Role");
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            ViewData["UserId"] = HttpContext.Session.GetInt32("UserId");
            if (_context.Promos != null)
            {
                Promo = await _context.Promos.ToListAsync();
            }

        }
    }
}

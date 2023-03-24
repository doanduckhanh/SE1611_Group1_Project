using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SE1611_Group1_Project.Models;

namespace SE1611_Group1_Project.Pages.Orders
{
    public class DetailsModel : PageModel
    {
        private readonly SE1611_Group1_Project.Models.FoodOrderContext _context;

        public DetailsModel(SE1611_Group1_Project.Models.FoodOrderContext context)
        {
            _context = context;
        }
        [BindProperty]
      public List<OrderDetail> orderDetails { get; set; } = default!;
        [BindProperty(SupportsGet = true)]
        public decimal? total { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? username { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime? date_created { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            ViewData["UserId"] = HttpContext.Session.GetInt32("UserId");
            ViewData["Role"] = HttpContext.Session.GetInt32("Role");
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                Response.Redirect("/Auth/Login");
            }
           
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            List<OrderDetail> orderdetail = _context.OrderDetails.Where(m => m.OrderId == id).Include(a => a.Food).ToList();
            if (orderdetail == null)
            {
                return NotFound();
            }
            else 
            {
                orderDetails = orderdetail;
            }
            total = _context.Orders.FirstOrDefault(m => m.OrderId == id).Total;
            username = _context.Orders.FirstOrDefault(m => m.OrderId == id).UserName;
            date_created = _context.Orders.FirstOrDefault(m => m.OrderId == id).OrderDate;
            return Page();
        }
    }
}

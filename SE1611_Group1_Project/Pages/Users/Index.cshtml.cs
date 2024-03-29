﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SE1611_Group1_Project.Models;

namespace SE1611_Group1_Project.Pages.Users
{
    public class IndexModel : PageModel
    {
        private readonly SE1611_Group1_Project.Models.FoodOrderContext _context;

        public IndexModel(SE1611_Group1_Project.Models.FoodOrderContext context)
        {
            _context = context;
        }

        public IList<User> User { get;set; } = default!;
        public int userId { get; set; } = default!;

        public async Task OnGetAsync()
        {
            
            ViewData["UserId"] = HttpContext.Session.GetInt32("UserId");
            ViewData["Role"] = HttpContext.Session.GetInt32("Role");
            ViewData["Username"] = HttpContext.Session.GetString("Username");

            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                Response.Redirect("/Auth/Login");
            }
            else if (HttpContext.Session.GetInt32("UserId") != null && HttpContext.Session.GetInt32("Role") != 1)
            {
                Response.Redirect("/Auth/403");
            }
            if (_context.Users != null)
            {
                User = await _context.Users
                .Include(u => u.Role).ToListAsync();
            }
        }
    }
}

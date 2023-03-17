using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SE1611_Group1_Project.Models;

namespace SE1611_Group1_Project.Pages.Auth
{
    public class ChangePasswordModel : PageModel
    {
        private readonly SE1611_Group1_Project.Models.FoodOrderContext _context;

        public ChangePasswordModel(SE1611_Group1_Project.Models.FoodOrderContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User User { get; set; } = default!;

        [BindProperty]
        public string OldPassword { get; set; }

        [BindProperty]
        public string ComparePassword { get; set; }

        public string MsgError { get; set; }
        public string Msg { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId");
            ViewData["Role"] = HttpContext.Session.GetInt32("Role");
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            ViewData["UserId"] = HttpContext.Session.GetInt32("UserId");
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user =  await _context.Users.FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }
            User = user;
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId");
            return Page();
        }

        public void CheckOldPass()
        {
            if (!OldPassword.Equals(ComparePassword, StringComparison.Ordinal))
            {
                MsgError = "Old password not valid!";
            }
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId");
            ViewData["Role"] = HttpContext.Session.GetInt32("Role");
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            ViewData["UserId"] = HttpContext.Session.GetInt32("UserId");
            CheckOldPass();

            if (!ModelState.IsValid)
            {
                return Page();
            }


            _context.Attach(User).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(User.UserId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            Msg = "Change password success!";
            return Page();
        }

        private bool UserExists(int id)
        {
          return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SE1611_Group1_Project.Models;
using System.ComponentModel.DataAnnotations;

namespace SE1611_Group1_Project.Pages.Login
{
    public class IndexModel : PageModel
    {   
        FoodOrderContext _context = new FoodOrderContext();

        [BindProperty]
        public string UserName { get; set; }
		[BindProperty]
		public string Password { get; set; }
		public string Msg { get; set; }

		public void OnGet()
        {
        }

		public async Task<IActionResult> OnPostAsync()
		{

			var user = _context.Users.FirstOrDefault(u => u.UserName == UserName && u.Password == Password);

			if (user != null)
			{
				if (UserName.Equals(user.UserName, StringComparison.Ordinal) && Password.Equals(user.Password, StringComparison.Ordinal))
				{
					HttpContext.Session.SetInt32("UserId", (int)user.UserId);
					HttpContext.Session.SetInt32("Role",  (int)user.RoleId);
					HttpContext.Session.SetString("Username", user.UserName);
                    HttpContext.Session.SetString("Password", user.Password);

                    return RedirectToPage("/Index");
				}
				else
				{
					Msg = "Invalid email or password.";
					return Page();
				}
			}
			else
			{
				Msg = "Invalid email or password.";
				return Page();
			}
		}

	}
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NuGet.Configuration;
using SE1611_Group1_Project.Models;
using SE1611_Group1_Project.Pages.Foods;
using SE1611_Group1_Project.Services;
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

                    //-----------------------------
                    SettingsCart.UserName = HttpContext.Session.GetString("Username");
                    SettingsCart.CartId = SettingsCart.UserName;
                    //MigrateCart();
                    HttpContext.Session.SetInt32("Count", new CartModel(_context).GetCount());
                    return RedirectToPage("/Foods/Index");
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

        public void MigrateCart()
        {
            var shoppingCart = _context.Carts.Where(c => c.CartId == SettingsCart.CartId).ToList();
            foreach (Cart item in shoppingCart)
            {
                Cart userCartItem = _context.Carts.FirstOrDefault(c => c.CartId == SettingsCart.UserName && c.FoodId == item.FoodId);
                if (userCartItem != null)
                {
                    userCartItem.Count += item.Count;
                    _context.Carts.Remove(item);
                }
                else
                {
                    item.CartId = SettingsCart.UserName;
                }
            }
            _context.SaveChanges();
            SettingsCart.CartId = SettingsCart.UserName;
        }

    }
}

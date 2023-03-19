using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NuGet.Configuration;
using SE1611_Group1_Project.Models;
using SE1611_Group1_Project.Services;

namespace SE1611_Group1_Project.Pages.Foods
{
    public class CartModel : PageModel
    {
        private readonly FoodOrderContext _context;
        public decimal total { get; set; }
        public int countItem { get; set; }

        public CartModel(FoodOrderContext context)
        {
            _context = context;
        }
        public IList<Cart> Cart { get; set; } = default!;
        public void OnGet()
        {
            ViewData["Role"] = HttpContext.Session.GetInt32("Role");
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            ViewData["UserId"] = HttpContext.Session.GetInt32("UserId");

            if (_context.Carts != null)
            {
                Cart = _context.Carts
                .Where(x => x.CartId.Equals(SettingsCart.CartId))
                .Include(a => a.Food)
                .Include(a => a.Food.Category).ToList();
            }
            total = GetTotal();
            countItem = GetCount();
            HttpContext.Session.SetInt32("Count", GetCount());
        }
        public decimal GetTotal()
        {
            // Multiply album price by count of that album to get
            // the current price for each of those albums in the cart
            // sum all album price totals to get the cart total
            decimal? total = (from cartItems in _context.Carts
                              where cartItems.CartId == SettingsCart.CartId
                              select cartItems.Count * cartItems.Food.FoodPrice).Sum();
            return total ?? 0;
        }
        public int GetCount()
        {
            // Get the count of each item in the cart and sum them up
            int? count = (from cartItems in _context.Carts
                          where cartItems.CartId == SettingsCart.CartId
                          select (int?)cartItems.Count).Count();
            // Return 0 if all entries are null
            return count ?? 0;

        }
        public async Task<IActionResult> OnPostRemoveFromCart(int id)
        {
            var cartItem = _context.Carts.SingleOrDefault(
                c => c.CartId.Equals(SettingsCart.CartId)
                && c.RecordId == id);
            if (cartItem != null)
            {
                if (cartItem.Count > 1)
                {
                    cartItem.Count--;
                }
                else
                {
                    _context.Carts.Remove(cartItem);
                }
                // Save changes
                await _context.SaveChangesAsync();

            }

            HttpContext.Session.SetInt32("Count", GetCount());

            return RedirectToPage("/Foods/Cart");
        }
        public async Task<IActionResult> OnPostRemove(int id)
        {
            var cartItem = _context.Carts.SingleOrDefault(
                c => c.CartId.Equals(SettingsCart.CartId)
                && c.RecordId == id);
            if (cartItem != null)
            {
                
                
                    _context.Carts.Remove(cartItem);
                
                // Save changes
                await _context.SaveChangesAsync();

            }

            HttpContext.Session.SetInt32("Count", GetCount());

            return RedirectToPage("/Foods/Cart");
        }
        public async Task<IActionResult> OnPostIncreaseFromCart(int id)
        {
            var cartItem = _context.Carts.SingleOrDefault(
                c => c.CartId == SettingsCart.CartId
                && c.RecordId == id);

            if (cartItem != null)
            {
                    cartItem.Count++;             
                // Save changes
                await _context.SaveChangesAsync();

            }

            HttpContext.Session.SetInt32("Count", GetCount());

            return RedirectToPage("/Foods/Cart");
        }
    }
}

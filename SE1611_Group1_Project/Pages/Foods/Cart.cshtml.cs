using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NuGet.Configuration;
using SE1611_Group1_Project.Models;
using SE1611_Group1_Project.Services;
using System.Numerics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SE1611_Group1_Project.Pages.Foods
{
    public class CartModel : PageModel
    {
        private readonly FoodOrderContext _context;
        [BindProperty(SupportsGet = true)]
        public decimal total { get; set; }
        public int countItem { get; set; }

        [BindProperty]
        public string Code { get; set; }
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
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                Response.Redirect("/Auth/Login");
            }
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
            HttpContext.Session.SetString("Total", total.ToString());
            ViewData["Total"] = HttpContext.Session.GetString("Total");
        }
        public decimal GetTotal()
        {
            // Multiply album price by count of that album to get
            // the current price for each of those albums in the cart
            // sum all album price totals to get the cart total
            decimal? total = (from cartItems in _context.Carts
                              where cartItems.CartId == SettingsCart.CartId
                              select cartItems.Count * cartItems.Food.FoodPrice).Sum();

            var promo = _context.Promos.SingleOrDefault(
                c => c.PromoCode.Equals(Code));

            if (promo != null)
            {
                if (promo.PromoValue.Contains('%'))
                {
                    string numStr = new string(promo.PromoValue.Where(c => Char.IsDigit(c)).ToArray());
                    int num = int.Parse(numStr);
                    var value = ((decimal)num / 100) * total;
                    total -= value;
                }
                if (Regex.IsMatch(promo.PromoValue, @"^\d+$"))
                {
                    total -= int.Parse(promo.PromoValue);
                }
                HttpContext.Session.SetString("CodePromo", Code.ToString());
            } else
            {
                HttpContext.Session.Remove("CodePromo");
            }
            
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

        public async Task<IActionResult> OnPostGiveCode()
        {
            // Retrieve session data
            ViewData["Role"] = HttpContext.Session.GetInt32("Role");
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            ViewData["UserId"] = HttpContext.Session.GetInt32("UserId");

            total = GetTotal();

            if (_context.Carts != null)
            {
                Cart = _context.Carts
                .Where(x => x.CartId.Equals(SettingsCart.CartId))
                .Include(a => a.Food)
                .Include(a => a.Food.Category).ToList();
            }
           
            countItem = GetCount();
            HttpContext.Session.SetInt32("Count", GetCount());

            HttpContext.Session.SetString("Total", total.ToString());
            
            ViewData["Total"] = HttpContext.Session.GetString("Total");

            var promo = _context.Promos.SingleOrDefault(
                c => c.PromoCode.Equals(Code));
            if(promo == null)
            {
                ViewData["MyString"] = "Coupon code has expired or is incorrect";
            }
            else
            {
                ViewData["MyString"] = "";
            }

            return Page();
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

        public async Task<IActionResult> OnPostCheckOut()
        {
           // HttpContext.Session.SetString("Total", total.ToString());
            List<Cart> carts = _context.Carts
                .Where(x => x.CartId.Equals(SettingsCart.CartId))
                .Include(a => a.Food)
                .Include(a => a.Food.Category).ToList();
            List<OrderDetailDTO> orderDetailDTOs = new List<OrderDetailDTO>();
            foreach (Cart cart in carts)
            {
                OrderDetailDTO orderDetailDTO = new OrderDetailDTO();
                orderDetailDTO.FoodId = cart.FoodId;
                orderDetailDTO.Quantity = cart.Count;
                orderDetailDTO.UnitPrice = cart.Food.FoodPrice;
                orderDetailDTOs.Add(orderDetailDTO);
            }
            HttpContext.Session.SetString("OrderDetailList", JsonSerializer.Serialize(orderDetailDTOs));
            return RedirectToPage("/Foods/Checkout");
        }
    }
}

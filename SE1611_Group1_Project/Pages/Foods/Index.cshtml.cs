using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SE1611_Group1_Project.Models;
using SE1611_Group1_Project.Services;
namespace SE1611_Group1_Project.Pages.Foods
{
    public class IndexModel : PageModel
    {
        public static FoodOrderContext context = new FoodOrderContext();
        PaginatedList<Food> foods = new PaginatedList<Food>(context.Foods.ToList(), context.Foods.Count(), 1, 3);

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        public SelectList? Categories { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CategoryId { get; set; } = 0;

        [BindProperty(SupportsGet = true)]
        public int IndexPaging { get; set; } = 1;

        [BindProperty(Name = "id", SupportsGet = true)]
        public int Id { get; set; }

        public int TotalPage { get; set; }
        public void OnGet(int categoryId, string searchString, int indexPaging)
        {
            /*ViewData["Role"] = HttpContext.Session.GetInt32("Role");
            ViewData["Username"] = HttpContext.Session.GetString("Username");*/
            ViewData["UserId"] = HttpContext.Session.GetInt32("UserId");
            ViewData["Role"] = HttpContext.Session.GetInt32("Role");
            ViewData["Username"] = HttpContext.Session.GetString("Username");

            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                Response.Redirect("/Auth/Login");
            }

            if (categoryId != 0)
            {
                var listFoods = String.IsNullOrEmpty(searchString) ? context.Foods.Where(x => x.CategoryId == categoryId).ToList() : context.Foods.Where(x => x.CategoryId == categoryId && x.FoodName.Contains(searchString)).ToList();
                foods = new PaginatedList<Food>(listFoods, listFoods.Count, 1, 6);
            }
            else
            {
                var listFoods = String.IsNullOrEmpty(searchString) ? context.Foods.ToList() : context.Foods.Where(x => x.FoodName.Contains(searchString)).ToList();
                foods = new PaginatedList<Food>(listFoods, listFoods.Count, 1, 6);
            }
            TotalPage = foods.TotalPages;
            ViewData["categoryList"] = context.Categories.ToList();
            ViewData["Product"] = PaginatedList<Food>.Create(foods.AsQueryable<Food>(), indexPaging, 6);
        }
        /*public IActionResult OnPostAddToCart(int id)
        {
            *//*var cartId = GetCartId();
            AddToCart(id, cartId);
            HttpContext.Session.SetInt32("Count", new CartModel(context).GetCount());
            return RedirectToPage("/Shopping/Cart");*//*
        }*/


        /*public static string GetCartId()
        {
            if (string.IsNullOrEmpty(Settings.CartId))
            {
                if (!string.IsNullOrEmpty(Settings.UserName))
                {
                    Settings.CartId = Settings.UserName;
                }

                else
                {
                    Guid tempCartId = Guid.NewGuid();
                    Settings.CartId = tempCartId.ToString();
                }
            }
            return Settings.CartId;
        }*/

        public void AddToCart(int albumId, String ShoppingCartId)
        {
           /* // Get the matching cart and album instances
            var cartItem = context.Carts.SingleOrDefault(
                c => c.CartId == ShoppingCartId
                && c.AlbumId == albumId);

            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists
                cartItem = new Cart
                {
                    AlbumId = albumId,
                    CartId = ShoppingCartId,
                    Count = 1,
                    DateCreated = DateTime.Now
                };

                context.Carts.Add(cartItem);
            }
            else
            {
                // If the item does exist in the cart, then add one to the quantity
                cartItem.Count++;
            }
            // Save changes
            context.SaveChanges();*/
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NuGet.Configuration;
using SE1611_Group1_Project.Models;
using SE1611_Group1_Project.Services;
using System.Diagnostics.Metrics;
using System.Net;
using System.Text.Json;

namespace SE1611_Group1_Project.Pages.Foods
{
    public class CheckoutModel : PageModel
    {
        FoodOrderContext foodOrderContext = new FoodOrderContext(); 
        [BindProperty(SupportsGet = true)]
        public decimal total { get; set; }
        [BindProperty(SupportsGet = true)]
        public string promocode { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<OrderDetailDTO> orderDetailDTOs { get; set; } = new List<OrderDetailDTO>();
        public void OnGet()
        {
            ViewData["Role"] = HttpContext.Session.GetInt32("Role");
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            ViewData["Total"] = HttpContext.Session.GetString("Total");
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                Response.Redirect("/Auth/Login");
            }

            orderDetailDTOs = JsonSerializer.Deserialize<List<OrderDetailDTO>>(HttpContext.Session.GetString("OrderDetailList"));
            if (HttpContext.Session.GetString("CodePromo") != null)
            {
                promocode = HttpContext.Session.GetString("CodePromo");
            }
            else
            {
                promocode = "";
            }
        }
        public async Task<IActionResult> OnPost()
        {
            orderDetailDTOs = JsonSerializer.Deserialize<List<OrderDetailDTO>>(HttpContext.Session.GetString("OrderDetailList"));
            Order order = new Order();
            order.OrderDate = DateTime.Now;
            if(HttpContext.Session.GetString("CodePromo") != null)
            {
                promocode = HttpContext.Session.GetString("CodePromo");
            } else
            {
                promocode = "";
            }
            order.PromoCode = promocode;
            order.UserName = HttpContext.Session.GetString("Username");
            total = Decimal.Parse(HttpContext.Session.GetString("Total"));
            order.Total = total;
            CreateOrder(order, orderDetailDTOs);
            return RedirectToPage("/Foods/Index");
        }
        public int CreateOrder(Order order, List<OrderDetailDTO> orderDetailDTOs)
        {

            decimal orderTotal = decimal.Parse(HttpContext.Session.GetString("Total"));
            // Set the order's total to the orderTotal count
            order.Total = orderTotal;
            // Save the order
            try
            {
                foodOrderContext.Orders.Add(order);
                foodOrderContext.SaveChanges();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                return -1;
            }
            int orderID = foodOrderContext.Orders.Select(o => o.OrderId).Max();
            // Iterate over the items in the cart, adding the order details for each
            foreach (OrderDetailDTO item in orderDetailDTOs)
            {
                var orderDetail = new OrderDetail
                {
                    FoodId = item.FoodId,
                    OrderId = orderID,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity
                };
                try
                {
                    foodOrderContext.OrderDetails.Add(orderDetail);
                    foodOrderContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                    return -1;
                }
            }
            // Empty the shopping cart
            EmptyCart();
            HttpContext.Session.SetInt32("Count", 0);
            // Return the OrderId as the confirmation number
            return orderID;
        }

        public void EmptyCart()
        {
            var cartItems = foodOrderContext.Carts
                .Where(cart => cart.CartId == SettingsCart.CartId);

            foreach (var cartItem in cartItems)
            {
                foodOrderContext.Carts.Remove(cartItem);
            }
            // Save changes
            foodOrderContext.SaveChanges();
        }
    }
}

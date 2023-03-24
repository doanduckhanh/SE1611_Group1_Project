using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SE1611_Group1_Project.Models;

namespace SE1611_Group1_Project.Pages.Statistics
{
    public class statisticsModel : PageModel
    {
        private readonly FoodOrderContext _context;

        public statisticsModel(FoodOrderContext context)
        {
            _context = context;
        }
        public SelectList? OrderDateYearView { get; set; }

        public SelectList? OrderDateMonthView { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? OrderDateYear { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? OrderDateMonth { get; set; }
        public void OnGet()
        {
            // Retrieve session data
            ViewData["Role"] = HttpContext.Session.GetInt32("Role");
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            ViewData["UserId"] = HttpContext.Session.GetInt32("UserId");
            //https://localhost:7154/Statistics/statistics
            IQueryable<string> orderDate = from m in _context.Orders
                                           orderby m.OrderDate descending
                                           select m.OrderDate.Value.Year.ToString();
            orderDate = orderDate.Distinct();
            List<string> orderDateList = orderDate.OrderByDescending(x => x).ToList();
            OrderDateYearView = new SelectList(orderDateList);

            IQueryable<string> orderDateMonth = from m in _context.Orders
                                                orderby m.OrderDate
                                                where m.OrderDate.Value.Year.Equals(int.Parse(orderDateList.First()))
                                                select m.OrderDate.Value.Month.ToString();
            orderDateMonth = orderDateMonth.Distinct();
            List<string> orderDateMonthList = orderDateMonth.OrderByDescending(x => x).ToList();
            OrderDateMonthView = new SelectList(orderDateMonthList, orderDateMonthList.First());



            var query = from od in _context.OrderDetails
                        join o in _context.Orders on od.OrderId equals o.OrderId
                        join f in _context.Foods on od.FoodId equals f.FoodId
                        where o.OrderDate.Value.Month == int.Parse(orderDateMonthList.First()) && o.OrderDate.Value.Year == int.Parse(orderDateList.First())
                        group od by new { f.FoodName, f.FoodPrice } into g
                        orderby g.Sum(od => od.Quantity) descending
                        select new
                        {
                            FoodName = g.Key.FoodName,
                            FoodPrice = g.Key.FoodPrice,
                            Quantity = g.Sum(od => od.Quantity)
                        };
            var top10 = query.Take(10);
            ViewData["Top10Data"] = top10;

            var queryTop10F = from od in _context.OrderDetails
                        join o in _context.Orders on od.OrderId equals o.OrderId
                        join f in _context.Foods on od.FoodId equals f.FoodId
                        where o.OrderDate.Value.Month == int.Parse(orderDateMonthList.First()) && o.OrderDate.Value.Year == int.Parse(orderDateList.First())
                        group od by new { f.FoodName, f.FoodPrice } into g
                        orderby g.Sum(od => od.Quantity) ascending
                        select new
                        {
                            FoodName = g.Key.FoodName,
                            FoodPrice = g.Key.FoodPrice,
                            Quantity = g.Sum(od => od.Quantity)
                        };
            var top10F = query.Take(10);
            ViewData["Top10DataF"] = top10F;



            //------------------------------------------------------------

            var queryv = from o in _context.Orders
                         join od in _context.OrderDetails on o.OrderId equals od.OrderId
                         join f in _context.Foods on od.FoodId equals f.FoodId
                         where o.OrderDate != null && o.OrderDate.Value.Year == int.Parse(orderDateList.First()) && o.OrderDate.Value.Month == int.Parse(orderDateMonthList.First())
                         group new { o, od, f } by o.OrderDate.Value.Day into g
                         select new DailyResult
                         {
                             Day = g.Key,
                             TotalRevenue = g.Sum(x => x.o.Total),
                             MostOrderedProduct = g.GroupBy(x => x.f.FoodName)
                                                   .Select(x => new { FoodName = x.Key, Quantity = x.Sum(y => y.od.Quantity) })
                                                   .OrderByDescending(x => x.Quantity)
                                                   .FirstOrDefault().FoodName ?? "N/A",
                             Quantity = g.GroupBy(x => x.f.FoodName)
                                          .Select(x => new { FoodName = x.Key, Quantity = x.Sum(y => y.od.Quantity) })
                                          .OrderByDescending(x => x.Quantity)
                                          .FirstOrDefault().Quantity ?? 0
                         };


            var dailyResults = queryv.ToList();
            var monthlyTotalSales = dailyResults.Sum(x => x.TotalRevenue);

            ViewData["DailyResults"] = dailyResults;
            ViewData["monthlyTotalSales"] = monthlyTotalSales;


            ViewData["Year"] = orderDateList.First();
            ViewData["Month"] = orderDateMonthList.First();

        }

        public async Task<IActionResult> OnPostSelected()
        {
            // Retrieve session data
            ViewData["Role"] = HttpContext.Session.GetInt32("Role");
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            ViewData["UserId"] = HttpContext.Session.GetInt32("UserId");


            IQueryable<string> orderDate = from m in _context.Orders
                                           orderby m.OrderDate descending
                                           select m.OrderDate.Value.Year.ToString();
            orderDate = orderDate.Distinct();
            List<string> orderDateList = orderDate.OrderByDescending(x => x).ToList();
            OrderDateYearView = new SelectList(orderDateList);

            //-------------------------------------------------------------------------------------
            string firstOrderDateYear = orderDateList.FirstOrDefault() ?? "0";
            //-------------------------------------------------------------------------------------

            
            //var listMont = _context.Orders.Where(x => x.OrderDate.Value.Year == int.Parse(OrderDateYear))
            //    .OrderByDescending(x => x.OrderDate.Value.Month).Select(x => x.OrderDate.Value.Month).Distinct().ToList();

            var listMont = _context.Orders
            .Where(x => x.OrderDate.Value.Year == int.Parse(OrderDateYear))
            .GroupBy(x => new { Year = x.OrderDate.Value.Year, Month = x.OrderDate.Value.Month })
            .OrderByDescending(x => x.Key.Month)
            .Select(x => x.Key.Month)
            .ToList();


            
            var listMontDefau = _context.Orders
            .Where(x => x.OrderDate.Value.Year == int.Parse(orderDateList.First()))
            .GroupBy(x => new { Year = x.OrderDate.Value.Year, Month = x.OrderDate.Value.Month })
            .OrderByDescending(x => x.Key.Month)
            .Select(x => x.Key.Month)
            .ToList();




            OrderDateMonthView = new SelectList(listMont, listMont.FirstOrDefault());
            //Console.WriteLine("---------------------------------------------------------");
            //Console.WriteLine("" + listMontDefau.First() + "-" + listMont.FirstOrDefault());
            var query = from od in _context.OrderDetails
                        join o in _context.Orders on od.OrderId equals o.OrderId
                        join f in _context.Foods on od.FoodId equals f.FoodId
                        where o.OrderDate.Value.Month == listMontDefau.FirstOrDefault() && o.OrderDate.Value.Year == int.Parse(orderDateList.First())
                        group od by new { f.FoodName, f.FoodPrice } into g
                        orderby g.Sum(od => od.Quantity) descending
                        select new
                        {
                            FoodName = g.Key.FoodName,
                            FoodPrice = g.Key.FoodPrice,
                            Quantity = g.Sum(od => od.Quantity)
                        };
            var top10 = query.Take(10);
            ViewData["Top10Data"] = top10;


            var queryTop10F = from od in _context.OrderDetails
                        join o in _context.Orders on od.OrderId equals o.OrderId
                        join f in _context.Foods on od.FoodId equals f.FoodId
                        where o.OrderDate.Value.Month == listMontDefau.FirstOrDefault() && o.OrderDate.Value.Year == int.Parse(orderDateList.First())
                        group od by new { f.FoodName, f.FoodPrice } into g
                        orderby g.Sum(od => od.Quantity) ascending
                        select new
                        {
                            FoodName = g.Key.FoodName,
                            FoodPrice = g.Key.FoodPrice,
                            Quantity = g.Sum(od => od.Quantity)
                        };
            var top10F = queryTop10F.Take(10);
            ViewData["Top10DataF"] = top10F;

            //------------------------------------------------------------

            var queryv = from o in _context.Orders
                         join od in _context.OrderDetails on o.OrderId equals od.OrderId
                         join f in _context.Foods on od.FoodId equals f.FoodId
                         where o.OrderDate != null && o.OrderDate.Value.Year == int.Parse(orderDateList.First()) && o.OrderDate.Value.Month == listMontDefau.FirstOrDefault()
                         group new { o, od, f } by o.OrderDate.Value.Day into g
                         select new DailyResult
                         {
                             Day = g.Key,
                             TotalRevenue = g.Sum(x => x.o.Total),
                             MostOrderedProduct = g.GroupBy(x => x.f.FoodName)
                                                   .Select(x => new { FoodName = x.Key, Quantity = x.Sum(y => y.od.Quantity) })
                                                   .OrderByDescending(x => x.Quantity)
                                                   .FirstOrDefault().FoodName ?? "N/A",
                             Quantity = g.GroupBy(x => x.f.FoodName)
                                          .Select(x => new { FoodName = x.Key, Quantity = x.Sum(y => y.od.Quantity) })
                                          .OrderByDescending(x => x.Quantity)
                                          .FirstOrDefault().Quantity ?? 0
                         };


            var dailyResults = queryv.ToList();
            var monthlyTotalSales = dailyResults.Sum(x => x.TotalRevenue);

            ViewData["DailyResults"] = dailyResults;
            ViewData["monthlyTotalSales"] = monthlyTotalSales;


            ViewData["Year"] = orderDateList.First();
            ViewData["Month"] = listMontDefau.First();  
            return Page();
        }

        public async Task<IActionResult> OnPostSubmit()
        {
            // Retrieve session data
            ViewData["Role"] = HttpContext.Session.GetInt32("Role");
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            ViewData["UserId"] = HttpContext.Session.GetInt32("UserId");

            
            Console.WriteLine("" + OrderDateYear + "-" + OrderDateMonth);

            IQueryable<string> orderDate = from m in _context.Orders
                                           orderby m.OrderDate descending
                                           select m.OrderDate.Value.Year.ToString();
            orderDate = orderDate.Distinct();
            List<string> orderDateList = orderDate.OrderByDescending(x => x).ToList();
            OrderDateYearView = new SelectList(orderDateList);

            //-------------------------------------------------------------------------------------
            string firstOrderDateYear = orderDateList.FirstOrDefault() ?? "0";
            //-------------------------------------------------------------------------------------


            //var listMont = _context.Orders.Where(x => x.OrderDate.Value.Year == int.Parse(OrderDateYear))
            //    .OrderByDescending(x => x.OrderDate.Value.Month).Select(x => x.OrderDate.Value.Month).Distinct().ToList();

            var listMont = _context.Orders
            .Where(x => x.OrderDate.Value.Year == int.Parse(OrderDateYear))
            .GroupBy(x => new { Year = x.OrderDate.Value.Year, Month = x.OrderDate.Value.Month })
            .OrderByDescending(x => x.Key.Month)
            .Select(x => x.Key.Month)
            .ToList();



            OrderDateMonthView = new SelectList(listMont, OrderDateMonth);



            var query = from od in _context.OrderDetails
                        join o in _context.Orders on od.OrderId equals o.OrderId
                        join f in _context.Foods on od.FoodId equals f.FoodId
                        where o.OrderDate.Value.Month == int.Parse(OrderDateMonth) && o.OrderDate.Value.Year == int.Parse(OrderDateYear)
                        group od by new { f.FoodName, f.FoodPrice } into g
                        orderby g.Sum(od => od.Quantity) descending
                        select new
                        {
                            FoodName = g.Key.FoodName,
                            FoodPrice = g.Key.FoodPrice,
                            Quantity = g.Sum(od => od.Quantity)
                        };
            var top10 = query.Take(10);
            ViewData["Top10Data"] = top10;

            var queryTop10F = from od in _context.OrderDetails
                        join o in _context.Orders on od.OrderId equals o.OrderId
                        join f in _context.Foods on od.FoodId equals f.FoodId
                        where o.OrderDate.Value.Month == int.Parse(OrderDateMonth) && o.OrderDate.Value.Year == int.Parse(OrderDateYear)
                        group od by new { f.FoodName, f.FoodPrice } into g
                        orderby g.Sum(od => od.Quantity) ascending
                        select new
                        {
                            FoodName = g.Key.FoodName,
                            FoodPrice = g.Key.FoodPrice,
                            Quantity = g.Sum(od => od.Quantity)
                        };
            var top10F = queryTop10F.Take(10);
            ViewData["Top10DataF"] = top10F;



            //----------------------------------------------------------------------------------------------------
            Console.WriteLine("---------------------------------------------------------");
            DateTime date = new DateTime(int.Parse(OrderDateYear), int.Parse(OrderDateMonth), 1);
            int daysInMonth = DateTime.DaysInMonth(int.Parse(OrderDateYear), int.Parse(OrderDateMonth));

            int year = 2024; // example year
            int month = 3; // example month

            //var queryv = from o in _context.Orders
            //            join od in _context.OrderDetails on o.OrderId equals od.OrderId
            //            join f in _context.Foods on od.FoodId equals f.FoodId
            //            where o.OrderDate != null && o.OrderDate.Value.Year == year && o.OrderDate.Value.Month == month
            //            group new { o, od, f } by o.OrderDate.Value.Day into g
            //            select new
            //            {
            //                Day = g.Key,
            //                TotalRevenue = g.Sum(x => x.o.Total),
            //                MostOrderedProduct = g.GroupBy(x => x.f.FoodName)
            //                                      .Select(x => new { FoodName = x.Key, Quantity = x.Sum(y => y.od.Quantity) })
            //                                      .OrderByDescending(x => x.Quantity)
            //                                      .FirstOrDefault()
            //            } into dailyResult
            //            orderby dailyResult.Day
            //            select dailyResult;

            //var monthlyTotalSales = queryv.Sum(x => x.TotalRevenue);

            var queryv = from o in _context.Orders
                         join od in _context.OrderDetails on o.OrderId equals od.OrderId
                         join f in _context.Foods on od.FoodId equals f.FoodId
                         where o.OrderDate != null && o.OrderDate.Value.Year == int.Parse(OrderDateYear) && o.OrderDate.Value.Month == int.Parse(OrderDateMonth)
                         group new { o, od, f } by o.OrderDate.Value.Day into g
                         select new DailyResult
                         {
                             Day = g.Key,
                             TotalRevenue = g.Sum(x => x.o.Total),
                             MostOrderedProduct = g.GroupBy(x => x.f.FoodName)
                                                   .Select(x => new { FoodName = x.Key, Quantity = x.Sum(y => y.od.Quantity) })
                                                   .OrderByDescending(x => x.Quantity)
                                                   .FirstOrDefault().FoodName ?? "N/A",
                             Quantity = g.GroupBy(x => x.f.FoodName)
                                          .Select(x => new { FoodName = x.Key, Quantity = x.Sum(y => y.od.Quantity) })
                                          .OrderByDescending(x => x.Quantity)
                                          .FirstOrDefault().Quantity ?? 0
                         };


            var dailyResults = queryv.ToList();
            var monthlyTotalSales = dailyResults.Sum(x => x.TotalRevenue);


            foreach (var dailyResult in queryv)
            {
                Console.WriteLine($"Day: {dailyResult.Day}, Total Revenue: {dailyResult.TotalRevenue:C2}, Most Ordered Product: {dailyResult.MostOrderedProduct ?? "None"}");
            }

            Console.WriteLine($"Total Sales for Month: {monthlyTotalSales:C2}");

            ViewData["DailyResults"] = dailyResults;
            ViewData["monthlyTotalSales"] = monthlyTotalSales;


            ViewData["Year"] = OrderDateYear;
            ViewData["Month"] = OrderDateMonth;

            return Page();
        }
    }

    public class DailyResult
    {
        public int? Day { get; set; }
        public decimal? TotalRevenue { get; set; }
        public string? MostOrderedProduct { get; set; }
        public int? Quantity { get; set; }
    }



}

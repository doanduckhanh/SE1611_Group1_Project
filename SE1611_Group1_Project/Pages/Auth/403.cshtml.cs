using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SE1611_Group1_Project.Pages.Auth
{
    public class _403Model : PageModel
    {
        public void OnGet()
        {
            ViewData["Role"] = HttpContext.Session.GetInt32("Role");
            ViewData["Username"] = HttpContext.Session.GetString("Username");

           
        }
    }
}

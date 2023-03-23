using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SE1611_Group1_Project.FileUploadService;
using SE1611_Group1_Project.Models;

namespace SE1611_Group1_Project.Pages.ManagementFoods
{
    public class CreateModel : PageModel
    {
        private readonly SE1611_Group1_Project.Models.FoodOrderContext _context;
        private readonly IFileUploadService fileUploadService;
        private readonly ILogger<IndexModel> _logger;
        public string filePath { get; set; } = default!;

        public CreateModel(SE1611_Group1_Project.Models.FoodOrderContext context,IFileUploadService fileUploadService, ILogger<IndexModel> logger)
        {
            _context = context;
            this.fileUploadService = fileUploadService;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
        ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return Page();
        }

        [BindProperty]
        public Food Food { get; set; } = default!;

        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {
          if (!ModelState.IsValid || _context.Foods == null || Food == null)
            {
                return Page();
            }
          if(Food.FoodPrice < 0)
            {
                ViewData["Message"] = string.Format("Price has to be more than 0!", Food.FoodPrice, DateTime.Now.ToString());
                return OnGet();
            }
          if(file!= null)
            {
                filePath = await fileUploadService.UploadFileAsync(file);
                Food.FoodImage = filePath.Substring(filePath.IndexOf(@"\images"));
            }
            else
            {
                return Page();
            }
                _context.Foods.Add(Food);
                await _context.SaveChangesAsync();


            return RedirectToPage("./Index");
        }
    }
}

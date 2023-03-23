using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SE1611_Group1_Project.FileUploadService;
using SE1611_Group1_Project.Models;

namespace SE1611_Group1_Project.Pages.ManagementFoods
{
    public class EditModel : PageModel
    {
        private readonly SE1611_Group1_Project.Models.FoodOrderContext _context;
        private readonly IFileUploadService fileUploadService;
        private readonly ILogger<IndexModel> _logger;

        public EditModel(SE1611_Group1_Project.Models.FoodOrderContext context, IFileUploadService fileUploadService, ILogger<IndexModel> logger)
        {
            _logger= logger;
            _context = context;
            this.fileUploadService = fileUploadService;
        }

        [BindProperty]
        public Food Food { get; set; } = default!;
        public List<Category> listCategories { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Foods == null)
            {
                return NotFound();
            }

            var food =  await _context.Foods.FirstOrDefaultAsync(m => m.FoodId == id);
            listCategories =  _context.Categories.ToList();
            if (food == null)
            {
                return NotFound();
            }
            Food = food;
           ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {
            if (file != null)
            {
                string filePath = await fileUploadService.UploadFileAsync(file);
                Food.FoodImage = filePath.Substring(filePath.IndexOf(@"\images"));
                _context.Foods.Update(Food);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            else
            {
                _context.Foods.Update(Food);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
           
        }

        private bool FoodExists(int id)
        {
          return (_context.Foods?.Any(e => e.FoodId == id)).GetValueOrDefault();
        }
    }
}

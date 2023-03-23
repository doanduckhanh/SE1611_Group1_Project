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
        public string filePath { get; set; } = default!;

        public EditModel(SE1611_Group1_Project.Models.FoodOrderContext context, IFileUploadService fileUploadService)
        {
            _context = context;
            this.fileUploadService = fileUploadService;
        }

        [BindProperty]
        public Food Food { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Foods == null)
            {
                return NotFound();
            }

            var food =  await _context.Foods.FirstOrDefaultAsync(m => m.FoodId == id);
            if (food == null)
            {
                return NotFound();
            }
            Food = food;
            filePath = Food.FoodImage;
           ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            filePath = Food.FoodImage;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Food).State = EntityState.Modified;

            try
            {
                if (file != null)
                {
                    filePath = await fileUploadService.UploadFileAsync(file);
                    Food.FoodImage = filePath.Substring(filePath.IndexOf(@"\images"));
                }
                else
                {
                    return Page();
                }
                _context.Foods.Update(Food);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FoodExists(Food.FoodId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool FoodExists(int id)
        {
          return (_context.Foods?.Any(e => e.FoodId == id)).GetValueOrDefault();
        }
    }
}

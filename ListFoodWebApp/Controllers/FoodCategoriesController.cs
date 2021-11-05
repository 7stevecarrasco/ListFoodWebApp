using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ListFoodWebApp.Data;
using ListFoodWebApp.Models;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ListFoodWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace ListFoodWebApp.Controllers
{
    
    public class FoodCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FoodCategoriesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: FoodCategories
        public async Task<IActionResult> Index()
        {
            var allCategories = await GetFoodCategoryTrees();
            return View(allCategories);
        }

        // GET: FoodCategories/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foodCategory = await _context.FoodCategories
                .FirstOrDefaultAsync(m => m.FoodCategoryID == id);
            if (foodCategory == null)
            {
                return NotFound();
            }

            return View(foodCategory);
        }

        // GET: FoodCategories/Create
        public async Task<IActionResult> Create()
        {
            var allCategories = await GetFoodCategoryTrees();
            var categoryNames = new List<SelectListItem>();

            foreach (var category in allCategories)
            {
                var items = GetDropdownItems(category);
                categoryNames.AddRange(items);
            }

            ViewBag.CategoryTree = categoryNames;

            var foodCategoryViewModel = new FoodCategory
            {
            };

            return PartialView("_CreateModal", foodCategoryViewModel);
        }

        // POST: FoodCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FoodCategoryID,ParentCategoryID,Name,Active")] FoodCategory foodCategory, IFormFile file)
        {
            if (file != null)
            {
                var fileExtension = Path.GetExtension(file.FileName)?.ToLower();
                if (fileExtension == ".jpg" || fileExtension == ".png" || fileExtension == ".jpeg")
                {
                    using (var stream = System.IO.File.Create(Path.GetExtension(file.FileName)?.ToLower()))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Please select a valid Image file");
                }
            }

            if (ModelState.IsValid)
            {
                foodCategory.FoodCategoryID = Guid.NewGuid();
                if (file != null)
                {
                    foodCategory.FoodCategoryImageName = UploadFile(file);
                }

                _context.Add(foodCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(foodCategory);
        }

        private string UploadFile(IFormFile file)
        {
            var uid = Guid.NewGuid();
            var fileName = $"{uid}_{file.FileName.Trim().Replace(" ", "_")}";
            var uploadedFolderLocation = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            if (!Directory.Exists(uploadedFolderLocation))
            {
                Directory.CreateDirectory(uploadedFolderLocation);
            }

            var filePath = Path.Combine(uploadedFolderLocation, fileName);
            using var fileStream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(fileStream);

            return fileName;
        }

        private void DeleteFile(string fileName)
        {
            if (System.IO.File.Exists(Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName)))
            {
                System.IO.File.Delete(Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName));
            }
        }

        // GET: FoodCategories/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foodCategory = await _context.FoodCategories.FindAsync(id);

            if (foodCategory == null)
            {
                return NotFound();
            }

            var allCategories = await GetFoodCategoryTrees();
            var categoryNames = new List<SelectListItem>();

            foreach (var category in allCategories)
            {
                var items = GetDropdownItems(category, id);
                categoryNames.AddRange(items);
            }

            ViewBag.CategoryTree = categoryNames;

            return PartialView("_EditModal", foodCategory);
        }

        // POST: FoodCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("FoodCategoryID,ParentCategoryID,Name,Active,FoodCategoryImageName")] FoodCategory foodCategory,
            IEnumerable<Guid> checkedCategoryIds, IFormFile file)
        {
            if (id != foodCategory.FoodCategoryID)
            {
                return NotFound();
            }
            if (file != null)
            {
                var fileExtension = Path.GetExtension(file.FileName)?.ToLower();
                if (fileExtension == ".jpg" || fileExtension == ".png" || fileExtension == ".jpeg")
                {
                }
                else
                {
                    ModelState.AddModelError("", "Please select a valid Image file");
                }
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (file != null)
                    {
                        if (!string.IsNullOrEmpty(foodCategory.FoodCategoryImageName))
                        {
                            DeleteFile(foodCategory.FoodCategoryImageName);
                        }

                        foodCategory.FoodCategoryImageName = UploadFile(file);
                    }
                    await SetUnactiveSubcategories(checkedCategoryIds, id);
                    _context.Update(foodCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FoodCategoryExists(foodCategory.FoodCategoryID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(foodCategory);
        }

        // GET: FoodCategories/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foodCategory = await _context.FoodCategories
                .FirstOrDefaultAsync(m => m.FoodCategoryID == id);
            if (foodCategory == null)
            {
                return NotFound();
            }

            return View(foodCategory);
        }

        private async Task SetUnactiveSubcategories(IEnumerable<Guid> checkedCategoryIds, Guid foodCategoryID)
        {
            var childCategories = _context.FoodCategories
                .Where(x => x.ParentCategoryID == foodCategoryID);

            foreach (var category in childCategories)
            {
                var dbCategory = await _context.FoodCategories.FirstAsync(x => x.FoodCategoryID == category.FoodCategoryID);

                if (checkedCategoryIds.Contains(dbCategory.FoodCategoryID))
                {
                    dbCategory.Active = true;
                }
                else
                {
                    dbCategory.Active = false;
                }
            }

            await _context.SaveChangesAsync();
        }

        // POST: FoodCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var foodCategory = await _context.FoodCategories.FindAsync(id);
            _context.FoodCategories.Remove(foodCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FoodCategoryExists(Guid id)
        {
            return _context.FoodCategories.Any(e => e.FoodCategoryID == id);
        }

        private List<SelectListItem> GetDropdownItems(FoodCategory category, Guid? excludeCategory = null)
        {
            var result = new List<SelectListItem>();

            var selectListItem = new SelectListItem()
            {
                Value = category.FoodCategoryID.ToString(),
                Text = GetFullNameForCategory(category)
            };

            if (excludeCategory == null || category.FoodCategoryID != excludeCategory)
            {
                result.Add(selectListItem);
            }

            if (category.Children.Any())
            {
                foreach (var loopCategory in category.Children)
                {
                    result.AddRange(GetDropdownItems(loopCategory, excludeCategory));
                }
            }

            return result;
        }

        private async Task<List<FoodCategory>> GetFoodCategoryTrees()
        {
            var categories = (
                await _context.FoodCategories
                    .Include(x => x.ParentCategory)
                    .Include(x => x.Children)
                    .ToListAsync()
                 )
                .Where(x => x.ParentCategoryID == null)
                .OrderBy(x => x.Name);

            return categories.ToList();
        }

        private string GetFullNameForCategory(FoodCategory category)
        {
            var result = new StringBuilder(category.Name);

            if (category.ParentCategory != null)
            {
                result = result.Insert(0, " - ");
                result = result.Insert(0, GetFullNameForCategory(category.ParentCategory));
            }

            return result.ToString();
        }

        public async Task<IActionResult> LoadProductByCategory(Guid id)
        {
            var itemOutput = new ProductViewModel();

            var foodCategory = await _context.FoodCategories.Where(x => x.ParentCategoryID == id).Select(x => new ProductSubCategory
            {
                ProductDescription = "Product Description",
                ProductName = x.Name,
                ProductId = x.FoodCategoryID,
                ProductActive = x.Active,
                FoodCategoryImageName = x.FoodCategoryImageName,
                Children = x.Children
            }).ToListAsync();

            var foodParentCategory = await _context.FoodCategories.FindAsync(id);

            itemOutput.ProductSubCategories = foodCategory;
            itemOutput.ProductParentName = foodParentCategory.Name;
            itemOutput.ProductParentId = foodParentCategory.FoodCategoryID;
            itemOutput.ProductParentActive = foodParentCategory.Active;
            itemOutput.ProductParentImageName = foodParentCategory.FoodCategoryImageName;

            return PartialView("_DisplayMainSection", itemOutput);
        }
    }
}

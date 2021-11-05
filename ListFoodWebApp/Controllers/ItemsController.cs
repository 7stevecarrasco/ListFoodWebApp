using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ListFoodWebApp.Data;
using ListFoodWebApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace ListFoodWebApp.Controllers
{
   
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Items
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Item.Include(i => i.FoodCategory);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Item
                .Include(i => i.FoodCategory)
                .FirstOrDefaultAsync(m => m.ItemID == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            ViewData["FoodCategoryID"] = new SelectList(_context.FoodCategories, "FoodCategoryID", "FoodCategoryID");

            var FoodCategoryList = new List<SelectListItem>();
            foreach (var m in _context.FoodCategories)
            {
                FoodCategoryList.Add(new SelectListItem
                {
                    Text = m.Name,
                    Value = m.FoodCategoryID.ToString()
                });
            }
            ViewBag.CategoryList = FoodCategoryList;
            return View();


        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemID,Upc_Code,Name,Calories,FoodCategoryID,Fiber,VitaminA,VitaminC,Protein,Calcium,Iron,Sodium,AddedSugar,SaturatedFat")] Item item)
        {
            if (ModelState.IsValid)
            {
                item.ItemID = Guid.NewGuid();
                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FoodCategoryID"] = new SelectList(_context.FoodCategories, "FoodCategoryID", "FoodCategoryID", item.FoodCategoryID);
            return View(item);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Item.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            ViewData["FoodCategoryID"] = new SelectList(_context.FoodCategories, "FoodCategoryID", "FoodCategoryID", item.FoodCategoryID);

            var FoodCategoryList = new List<SelectListItem>();
            foreach (var m in _context.FoodCategories)
            {
                FoodCategoryList.Add(new SelectListItem
                {
                    Text = m.Name,
                    Value = m.FoodCategoryID.ToString()
                });
            }
            ViewBag.CategoryList = FoodCategoryList;
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ItemID,Upc_Code,Name,Calories,FoodCategoryID,Fiber,VitaminA,VitaminC,Protein,Calcium,Iron,Sodium,AddedSugar,SaturatedFat")] Item item)
        {
            if (id != item.ItemID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.ItemID))
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
            ViewData["FoodCategoryID"] = new SelectList(_context.FoodCategories, "FoodCategoryID", "FoodCategoryID", item.FoodCategoryID);
            return View(item);
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Item
                .Include(i => i.FoodCategory)
                .FirstOrDefaultAsync(m => m.ItemID == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var item = await _context.Item.FindAsync(id);
            _context.Item.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(Guid id)
        {
            return _context.Item.Any(e => e.ItemID == id);
        }
    }
}

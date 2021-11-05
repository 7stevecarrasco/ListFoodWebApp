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
    
    public class OrderedItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderedItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: OrderedItems
        public async Task<IActionResult> Index()
        {


            return View(await _context.OrderedItems.ToListAsync());
        }

        // GET: OrderedItems/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderedItem = await _context.OrderedItems
                .FirstOrDefaultAsync(m => m.OrderedItemId == id);
            if (orderedItem == null)
            {
                return NotFound();
            }

            return RedirectToAction("Details", "ItemsController");
        }

        // GET: OrderedItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: OrderedItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderedItemId,OrderId,FoodCategoryId,Quantity")] OrderedItem orderedItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderedItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(orderedItem);
        }

        // GET: OrderedItems/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderedItem = await _context.OrderedItems.FindAsync(id);
            if (orderedItem == null)
            {
                return NotFound();
            }
            return View(orderedItem);
        }

        // POST: OrderedItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("OrderedItemId,OrderId,FoodCategoryId,Quantity")] OrderedItem orderedItem)
        {
            if (id != orderedItem.OrderedItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderedItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderedItemExists(orderedItem.OrderedItemId))
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
            return View(orderedItem);
        }

        // GET: OrderedItems/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderedItem = await _context.OrderedItems
                .FirstOrDefaultAsync(m => m.OrderedItemId == id);
            if (orderedItem == null)
            {
                return NotFound();
            }

            return View(orderedItem);
        }

        // POST: OrderedItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var orderedItem = await _context.OrderedItems.FindAsync(id);
            _context.OrderedItems.Remove(orderedItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderedItemExists(Guid id)
        {
            return _context.OrderedItems.Any(e => e.OrderedItemId == id);
        }
    }
}

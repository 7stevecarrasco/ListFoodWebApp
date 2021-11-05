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
   
    public class PurchaseItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchaseItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PurchaseItems
        public async Task<IActionResult> Index()
        {
            var purchaseItemContext = _context.PurchaseItems;

            return View(await purchaseItemContext.ToListAsync());
        }

        // GET: PurchaseItems/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseItem = await _context.PurchaseItems
                .FirstOrDefaultAsync(m => m.PurchaseItemID == id);
            if (purchaseItem == null)
            {
                return NotFound();
            }

            return View(purchaseItem);
        }

        // GET: PurchaseItems/Create
        public IActionResult Create()
        {
            var itemList = new List<SelectListItem>();
            foreach (var m in _context.Item)
            {
                itemList.Add(new SelectListItem
                {
                    Text = m.Name,
                    Value = m.ItemID.ToString()
                });

            }

            ViewBag.ListofItems = itemList;
            return View();
        }

        // POST: PurchaseItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PurchaseItemID,PurchaseDate,ExpirationDate,Cost,ItemID")] PurchaseItem purchaseItem)
        {
            if (ModelState.IsValid)
            {
                // purchaseItem.PurchaseItemID = int.Ne();
                _context.Add(purchaseItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(purchaseItem);
        }

        // GET: PurchaseItems/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseItem = await _context.PurchaseItems.FindAsync(id);
            if (purchaseItem == null)
            {
                return NotFound();
            }

            var itemList = new List<SelectListItem>();
            foreach (var m in _context.Item)
            {
                itemList.Add(new SelectListItem
                {
                    Text = m.Name,
                    Value = m.ItemID.ToString()
                });

            }

            ViewBag.ListofItems = itemList;

            return View(purchaseItem);
        }

        // POST: PurchaseItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("PurchaseItemID,PurchaseDate,ExpirationDate,Cost,ItemID")] PurchaseItem purchaseItem)
        {
            if (id != purchaseItem.PurchaseItemID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(purchaseItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseItemExists(purchaseItem.PurchaseItemID))
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
            return View(purchaseItem);
        }

        // GET: PurchaseItems/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseItem = await _context.PurchaseItems
                .FirstOrDefaultAsync(m => m.PurchaseItemID == id);
            if (purchaseItem == null)
            {
                return NotFound();
            }

            return View(purchaseItem);
        }

        // POST: PurchaseItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var purchaseItem = await _context.PurchaseItems.FindAsync(id);
            _context.PurchaseItems.Remove(purchaseItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PurchaseItemExists(Guid id)
        {
            return _context.PurchaseItems.Any(e => e.PurchaseItemID == id);
        }
    }
}

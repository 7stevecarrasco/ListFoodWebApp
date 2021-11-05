using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ListFoodWebApp.Data;
using ListFoodWebApp.Models;
using ListFoodWebApp.Models.ViewModels;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace ListFoodWebApp.Controllers
{
    
    public class ShoppingListController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShoppingListController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ShoppingList
        public async Task<IActionResult> Index()
        {
            DateTime dateTime = DateTime.Parse("12/24/2021 12:00:00 AM");

            var purchasedList = (from oi in _context.OrderedItems
                                 join o in _context.Orders on oi.OrderId equals o.OrderID
                                 where o.Status == Order.StatusType.ListSubmitted
                                 join p in _context.PurchasedList on oi.OrderedItemId equals p.OrderedItemID into purchases
                                 from purchaseOutter in purchases.DefaultIfEmpty()
                                 group purchaseOutter by new
                                 {
                                     FoodCategoryId = oi.FoodCategoryId
                                 } into ordereditemsGroup
                                 select new
                                 {
                                     FoodCategoryId = ordereditemsGroup.Key.FoodCategoryId,
                                     PurchasedQty = ordereditemsGroup.Sum(l => l.Quantity == null ? 0 : l.Quantity)
                                 }
             ).ToList();

            var shoppingList = (from fc in _context.FoodCategories
                                join oi in _context.OrderedItems on fc.FoodCategoryID equals oi.FoodCategoryId
                                join o in _context.Orders on oi.OrderId equals o.OrderID
                                where o.Status == Order.StatusType.ListSubmitted
                                group oi by new
                                {
                                    FoodCategoryID = oi.FoodCategoryId,
                                    FoodIcon = fc.FoodCategoryImageName,
                                    FoodName = fc.Name,
                                    FulfillStatus = "Unfulfilled"
                                } into orderItemsGrouped
                                select new ShoppingList
                                {
                                    FoodName = orderItemsGrouped.Key.FoodName,
                                    FoodIcon = orderItemsGrouped.Key.FoodIcon,
                                    FoodCategoryID = orderItemsGrouped.Key.FoodCategoryID,
                                    Quantity = orderItemsGrouped.Sum(l => l.Quantity),
                                    FulfillStatus = orderItemsGrouped.Key.FulfillStatus,
                                }
                             ).ToList();
            for (int i = 0; i < shoppingList.Count; i++)
            {
                var foodItem = shoppingList[i];
                shoppingList[i].FulfillStatus = foodItem.Quantity <=
                                                (purchasedList
                                                    .Where(p => p.FoodCategoryId == foodItem.FoodCategoryID)
                                                    .FirstOrDefault()
                                                ).PurchasedQty ? "Fulfilled" : "Unfulfilled";

            }

            return View(shoppingList);
        }

        // GET: ShoppingList/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderedItem = await _context.OrderedItems
                .Include(o => o.FoodCategory)
                .Include(o => o.Order)
                .FirstOrDefaultAsync(m => m.OrderedItemId == id);
            if (orderedItem == null)
            {
                return NotFound();
            }

            return View(orderedItem);
        }

        // GET: ShoppingList/Create
        public IActionResult Create()
        {
            ViewData["FoodCategoryId"] = new SelectList(_context.FoodCategories, "FoodCategoryID", "FoodCategoryID");
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderID", "OrderID");
            return View();
        }

        // POST: ShoppingList/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderedItemId,OrderId,FoodCategoryId,Quantity")] OrderedItem orderedItem)
        {
            if (ModelState.IsValid)
            {
                orderedItem.OrderedItemId = Guid.NewGuid();
                _context.Add(orderedItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FoodCategoryId"] = new SelectList(_context.FoodCategories, "FoodCategoryID", "FoodCategoryID", orderedItem.FoodCategoryId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderID", "OrderID", orderedItem.OrderId);
            return View(orderedItem);
        }

        // GET: ShoppingList/Edit/5
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
            ViewData["FoodCategoryId"] = new SelectList(_context.FoodCategories, "FoodCategoryID", "FoodCategoryID", orderedItem.FoodCategoryId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderID", "OrderID", orderedItem.OrderId);
            return View(orderedItem);
        }

        // POST: ShoppingList/Edit/5
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
            ViewData["FoodCategoryId"] = new SelectList(_context.FoodCategories, "FoodCategoryID", "FoodCategoryID", orderedItem.FoodCategoryId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderID", "OrderID", orderedItem.OrderId);
            return View(orderedItem);
        }

        // GET: ShoppingList/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderedItem = await _context.OrderedItems
                .Include(o => o.FoodCategory)
                .Include(o => o.Order)
                .FirstOrDefaultAsync(m => m.OrderedItemId == id);
            if (orderedItem == null)
            {
                return NotFound();
            }

            return View(orderedItem);
        }

        // POST: ShoppingList/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var orderedItem = await _context.OrderedItems.FindAsync(id);
            _context.OrderedItems.Remove(orderedItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: ShoppingList/Fulfill/

        public IActionResult Fulfill(Guid? id)
        {
            List<FoodCategory> foodCategories = _context.FoodCategories.Where(f => f.FoodCategoryID == id.Value).ToList();
            List<OrderedItem> orderedItems = _context.OrderedItems.ToList();

            var categoryList = new List<SelectListItem>();
            foreach (var b in foodCategories)
            {
                categoryList.Add(new SelectListItem
                {
                    Text = b.Name,
                    Value = b.FoodCategoryID.ToString()
                });
            }
            FoodCategory food;
            ViewBag.ListofCategories = categoryList;
            ShoppingItem svm = new ShoppingItem();
            svm.PurchaseDate = DateTime.UtcNow;

            if (foodCategories.Any())
            {
                food = foodCategories.ElementAt(0);
                svm.FoodIcon = food.FoodCategoryImageName;
                svm.FoodName = food.Name;
            }

            return View(svm);
        }

        // POST: ShoppingList/Fulfill
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Fulfill([Bind("Upc_Code, Quantity, Name, Calories, FoodCategoryID, " +
            "Fiber, VitaminA, VitaminC, Protein, Calcium, Iron, Sodium, AddedSugar, SaturatedFat")] ShoppingItem shoppingList)
        {
            Item item = _context.Item.Where(x => x.Upc_Code == shoppingList.Upc_Code).FirstOrDefault();

            if (item == null && shoppingList.Quantity == 0)
            {
                ModelState.AddModelError("Upc_Code", "UPC Code not Found, please enter nutritional info manually");
                return View(shoppingList);
            }

            if (item == null)
            {
                try
                {
                    Item i = new Item()
                    {
                        ItemID = Guid.NewGuid(),
                        Upc_Code = shoppingList.Upc_Code,
                        Name = shoppingList.Name,
                        Calories = shoppingList.Calories,
                        FoodCategoryID = shoppingList.FoodCategoryID,
                        Fiber = shoppingList.Fiber,
                        VitaminA = shoppingList.VitaminA,
                        VitaminC = shoppingList.VitaminC,
                        Protein = shoppingList.Protein,
                        Calcium = shoppingList.Calcium,
                        Iron = shoppingList.Iron,
                        Sodium = shoppingList.Sodium,
                        AddedSugar = shoppingList.AddedSugar,
                        SaturatedFat = shoppingList.SaturatedFat
                    };
                    _context.Add(i);
                    item = i;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderedItemExists(shoppingList.OrderedItemID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            PurchaseItem purchaseItem = new PurchaseItem()
            {
                PurchaseItemID = Guid.NewGuid(),
                PurchaseDate = DateTime.UtcNow,
                ExpirationDate = shoppingList.ExpirationDate,
                Cost = shoppingList.Cost,
                ItemID = item.ItemID
            };
            _context.Add(purchaseItem);
            await _context.SaveChangesAsync();

            var items = (from oi in _context.OrderedItems
                         join p in _context.PurchasedList on oi.OrderedItemId equals p.OrderedItemID into purchases
                         from purchaseOutter in purchases.DefaultIfEmpty()
                         where oi.FoodCategoryId == shoppingList.FoodCategoryID
                         group purchaseOutter by new
                         {
                             OrderedItemId = oi.OrderedItemId,
                             FoodCategoryId = oi.FoodCategoryId,
                             FulfillQty = oi.Quantity
                         } into ordereditemsGroup
                         select new
                         {
                             OrderedItemId = ordereditemsGroup.Key.OrderedItemId,
                             FoodCategoryId = ordereditemsGroup.Key.FoodCategoryId,
                             FulfillQty = ordereditemsGroup.Key.FulfillQty - ordereditemsGroup.Sum(l => l.Quantity == null ? 0 : l.Quantity)
                         }
                         ).ToList();

            var test = items;

            var totalQty = shoppingList.Quantity;

            for (int i = 0; i < items.Count; i++)
            {
                var oItem = items[i];
                PurchasedList purchaseList = new PurchasedList()
                {
                    PurchasedListID = Guid.NewGuid(),
                    OrderedItemID = oItem.OrderedItemId,
                    PurchaseItemID = purchaseItem.PurchaseItemID,
                    PurchasedItemID = purchaseItem.PurchaseItemID,
                    Quantity = (totalQty >= oItem.FulfillQty) ? oItem.FulfillQty : totalQty
                };
                _context.Add(purchaseList);
                await _context.SaveChangesAsync();
                totalQty = totalQty - oItem.FulfillQty;
                if (totalQty <= 0)
                {
                    break;
                }
            }
            if (totalQty > 0 && items.Count > 0)
            {
                PurchasedList purchaseList = new PurchasedList()
                {
                    PurchasedListID = Guid.NewGuid(),
                    OrderedItemID = items[0].OrderedItemId,
                    PurchaseItemID = purchaseItem.PurchaseItemID,
                    PurchasedItemID = purchaseItem.PurchaseItemID,
                    Quantity = totalQty
                };
                _context.Add(purchaseList);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool OrderedItemExists(Guid id)
        {
            return _context.OrderedItems.Any(e => e.OrderedItemId == id);
        }
    }
}

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
using ListFoodWebApp.Models.ViewModels;
using Nancy.Json;

namespace ListFoodWebApp.Controllers
{
    
    public class ViewOrders : Controller
    {
        private readonly ApplicationDbContext _context;

        public ViewOrders(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Orders.Include(o => o.Event).Include(o => o.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderedItemsDetails = await (from oi in _context.OrderedItems
                                             join o in _context.Orders on oi.OrderId equals o.OrderID
                                             join fc in _context.FoodCategories on oi.FoodCategoryId equals fc.FoodCategoryID
                                             where oi.OrderId == id
                                             select new ShoppingList
                                             {
                                                 FoodIcon = fc.FoodCategoryImageName,
                                                 FoodName = fc.Name,
                                                 Quantity = oi.Quantity,
                                                 OrderID = oi.OrderId
                                             }).ToListAsync();

            var order = _context.Orders.Where(o => o.OrderID == id).FirstOrDefault();

            ViewBag.Status = order.Status;
            ViewBag.OrderId = id;

            var statusList = new List<SelectListItem>();
            statusList.Add(new SelectListItem
            {
                Text = "List Submitted",
                Value = Order.StatusType.ListSubmitted.ToString()
            });
            statusList.Add(new SelectListItem
            {
                Text = "Finished List",
                Value = Order.StatusType.FinishedList.ToString()
            });
            statusList.Add(new SelectListItem
            {
                Text = "List Cancelled",
                Value = Order.StatusType.ListCancelled.ToString()
            });
           
            


            ViewBag.StatusList = statusList;

            if (orderedItemsDetails == null)
            {
                return NotFound();
            }

            return View(orderedItemsDetails);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateOrderStatus([FromBody] object orderIdAndStatus)
        {
            OrderStatusVM vm = new JavaScriptSerializer().Deserialize<OrderStatusVM>(orderIdAndStatus.ToString());

            Order orderQry = _context.Orders.Where(o => o.OrderID == vm.OrderID).FirstOrDefault();

            try
            {
                orderQry.Status = (Order.StatusType)vm.Status;
                _context.Update(orderQry);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(vm.OrderID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Json(orderIdAndStatus);
        }

        private bool OrderExists(Guid id)
        {
            return _context.Orders.Any(e => e.OrderID == id);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["EventID"] = new SelectList(_context.Events, "EventId", "EventId");
            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderID,UserId,EventID,DateOrdered,PickupDate,Status,Generic")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.OrderID = Guid.NewGuid();
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            ViewData["EventID"] = new SelectList(_context.Events, "EventId", "EventId", order.EventID);
            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", order.UserId);
            //ViewData["Title"] = new SelectList(_context.Events, "Title", "Title", )
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["EventID"] = new SelectList(_context.Events, "EventId", "EventId", order.EventID);
            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", order.UserId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("OrderID,UserId,EventID,DateOrdered,PickupDate,Status,Generic")] Order order)
        {
            if (id != order.OrderID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderID))
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
            ViewData["EventID"] = new SelectList(_context.Events, "EventId", "EventId", order.EventID);
            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", order.UserId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Event)
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
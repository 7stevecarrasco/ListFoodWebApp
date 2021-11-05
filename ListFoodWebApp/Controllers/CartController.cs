using ListFoodWebApp.Data;
using ListFoodWebApp.Models;
using ListFoodWebApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ListFoodWebApp.Controllers
{
   
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CartController
        public ActionResult Index()
        {

            var orderedItems = _context.OrderedItems //grab the current ordered items that belong to the user currently in session
                                           .Where(o => o.Order.Status == Order.StatusType.ListSubmitted && o.Order.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                                           .Include(o => o.FoodCategory)
                                           .ToList();
            var cartItems = new List<CartItem>();

            foreach (var item in orderedItems)
            {
                var cartItem = new CartItem();
                var foodCategory = _context.FoodCategories.Where(x => x.FoodCategoryID == item.FoodCategoryId).Select(x => x.FoodCategoryImageName).FirstOrDefault();
                cartItem.FoodCategoryId = item.FoodCategoryId;
                cartItem.FoodCategory = item.FoodCategory;
                cartItem.FoodCategoryImageName = foodCategory;
                cartItem.OrderedItemId = item.OrderedItemId;
                cartItem.OrderId = item.OrderId;
                cartItem.Order = item.Order;
                cartItem.Quantity = item.Quantity;
                cartItems.Add(cartItem);
            }
            return View(cartItems);
        }

        // GET: CartController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CartController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CartController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CartController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CartController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CartController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CartController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public IActionResult SelectEvent()
        {
            var orderedItems = _context.OrderedItems //grab the current ordered items that belong to the user currently in session
                                           .Where(o => o.Order.Status == Order.StatusType.ListSubmitted && o.Order.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                                           .Include(o => o.FoodCategory)
                                           .ToList();
            var cartItems = new List<CartItem>();
            var events = (from e in _context.Events
                          join o in _context.Orders on e.EventId equals o.EventID
                          where orderedItems.Select(oi => oi.OrderId).ToList().Contains(o.OrderID)
                          select e).ToList();
            ViewBag.Events = events;

            return PartialView("_SelectEvent");
        }
        public IActionResult LoadEventInfo(Guid id)
        {
            var selectedEvent = _context.Events.Where(o => o.EventId == id).FirstOrDefault();

            ViewBag.selectEvent = selectedEvent;

            return PartialView("_LoadEventInfo", selectedEvent);
        }

        public async Task<ActionResult> OrderConfirmation()
        {
            var order = _context.Orders.Where(o => o.Status == Order.StatusType.FinishedList && o.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier)).SingleOrDefault();
            order.Status = Order.StatusType.ListSubmitted;

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
            return View("OrderConfirmation", order);
        }
        public IActionResult GetAllOrders()
        {
            var orders = _context.Orders
                               .Where(o => o.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                               .ToList();

            return PartialView("_UserOrders", orders);
        }
        private bool OrderExists(Guid id)
        {
            return _context.Orders.Any(e => e.OrderID == id);
        }
    }
}

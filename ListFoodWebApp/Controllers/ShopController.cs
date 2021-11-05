using ListFoodWebApp.Data;
using ListFoodWebApp.Models;
using ListFoodWebApp.Models.ViewModels;
using ListFoodWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nancy.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ListFoodWebApp.Controllers
{
   
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShopController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Shop
        public async Task<IActionResult> Index()
        {
            var nextEvent = _context.Events.Where(o => o.StartDate > DateTime.Today).ToList();
            var open = true;
            if (nextEvent.Count == 0)
            {
                open = false;
            }
            else
            {
                var EventDate = nextEvent.ElementAt(0).StartDate;

                if (EventDate.AddDays(-3) <= DateTime.Now)
                {
                    open = false;
                }
            }
            ViewBag.Open = open;

            var allCategories = await GetParentTrees();
            return View(allCategories);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] object duBlueCow)
        {
            ShopVM itemToCart = new JavaScriptSerializer().Deserialize<ShopVM>(duBlueCow.ToString());
            var categoryId = itemToCart.Id;

            var order = _context.Orders
                                   .Where(o => o.Status == Order.StatusType.FinishedList && o.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier)).SingleOrDefault();

            var category = _context.FoodCategories.Where(o => o.FoodCategoryID == categoryId).SingleOrDefault();

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            
            
            

            if (order == null)
            {
                var nextEvent = _context.Events.Where(o => o.StartDate > DateTime.Today).ToList();

                Order newOrder = new Order();
                newOrder.OrderID = new Guid();
                newOrder.Status = Order.StatusType.ListSubmitted;
                newOrder.EventID = nextEvent.ElementAt(0).EventId;
                newOrder.PickupDate = nextEvent.ElementAt(0).StartDate;
                newOrder.DateOrdered = DateTime.Today;
                newOrder.Generic = false;
             

                OrderedItem newItem = new OrderedItem();
                newItem.OrderedItemId = new Guid();
                newItem.Order = newOrder;
                newItem.OrderId = newOrder.OrderID;
                newItem.FoodCategory = category;
                newItem.FoodCategoryId = category.FoodCategoryID;
                newItem.Quantity = itemToCart.Amount;

                _context.Add(newOrder);
                _context.Add(newItem);
            }
            else
            {
                if (categoryInOrder(categoryId, order.OrderID))
                {
                    var orderedItem = _context.OrderedItems.Where(o => o.OrderId == order.OrderID && o.FoodCategoryId == categoryId).SingleOrDefault();
                    orderedItem.Quantity += itemToCart.Amount;
                    _context.Update(orderedItem);
                }
                else
                {

                    OrderedItem newItem = new OrderedItem();
                    newItem.OrderedItemId = new Guid();
                    newItem.Order = order;
                    newItem.OrderId = order.OrderID;
                    newItem.FoodCategory = category;
                    newItem.FoodCategoryId = category.FoodCategoryID;
                    newItem.Quantity = itemToCart.Amount;

                    _context.Add(newItem);
                }
            }
            await _context.SaveChangesAsync();

            return Json(duBlueCow);
        }
        [HttpPost]
        public async Task<PartialViewResult> AddPartialToViewAsync(string id)
        {
            var categoryId = Guid.Parse(id);
            var categories = (
                await _context.FoodCategories
                    .Include(x => x.ParentCategory)
                    .Include(x => x.Children)
                    .ToListAsync()
                 )
                .Where(x => x.ParentCategoryID == categoryId)
                .OrderBy(x => x.Name);

            return PartialView("_ChildrenTreeView", categories.ToList());
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

        public async Task<IActionResult> AddToCartModal(Guid? id)
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

            return PartialView("_AddToCart", foodCategory);
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

            var nextEvent = _context.Events.Where(o => o.StartDate > DateTime.Today).ToList();
            var open = true;
            if (nextEvent.Count == 0)
            {
                open = false;
            }
            else
            {
                var EventDate = nextEvent.ElementAt(0).StartDate;

                if (EventDate.AddDays(-3) <= DateTime.Now)
                {
                    open = false;
                }
            }
            ViewBag.Open = open;

            return PartialView("_DisplayMainSection", itemOutput);
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

        private async Task<List<FoodCategory>> GetParentTrees()
        {
            var categories = (
                await _context.FoodCategories
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

        private bool categoryInOrder(Guid catId, Guid orderId)
        {
            return _context.OrderedItems.Any(e => e.OrderId == orderId && e.FoodCategoryId == catId);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ListFoodWebApp.Models.ViewModels
{
    public class ShoppingList
    {
        public string FoodName { get; set; }
        public string FulfillStatus { get; set; }
        public string FoodIcon { get; set; }
        public Guid OrderedItemID { get; set; }
        public Guid OrderID { get; set; }
        public int Quantity { get; set; }
        public int? PurchaseListQty { get; set; }
        public Guid FoodCategoryID { get; internal set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ListFoodWebApp.Models.ViewModels
{
    public class CartItem
    {
        public Guid OrderedItemId { get; set; }
        public Guid OrderId { get; set; }
        public Guid FoodCategoryId { get; set; }
        public string FoodCategoryImageName { get; set; }
        public int Quantity { get; set; }
        public virtual Order Order { get; set; }
        public virtual FoodCategory FoodCategory { get; set; }
    }
}

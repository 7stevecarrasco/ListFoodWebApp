using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ListFoodWebApp.Models
{
    public class OrderedItem
    {
        public Guid OrderedItemId { get; set; }
        public Guid OrderId { get; set; }
        public Guid FoodCategoryId { get; set; }
        public int Quantity { get; set; }
        public virtual Order Order { get; set; }
        public virtual FoodCategory FoodCategory { get; set; }
    }
}

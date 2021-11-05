using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ListFoodWebApp.Models.ViewModels
{
    public class ShoppingItem
    {
        public string FoodName { get; set; }
        public string FulfillStatus { get; set; }
        public string FoodIcon { get; set; }
        public Guid OrderedItemID { get; set; }
        public Guid OrderID { get; set; }
        public Guid ItemID { get; set; }
        public string Upc_Code { get; set; }
        public string Name { get; set; }
        public int Calories { get; set; }
        public Guid FoodCategoryID { get; set; }
        public int Fiber { get; set; }
        public int VitaminA { get; set; }
        public int VitaminC { get; set; }
        public int Protein { get; set; }
        public int Calcium { get; set; }
        public int Iron { get; set; }
        public int Sodium { get; set; }
        public int AddedSugar { get; set; }
        public int SaturatedFat { get; set; }
        public int Quantity { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public double Cost { get; set; }
    }
}

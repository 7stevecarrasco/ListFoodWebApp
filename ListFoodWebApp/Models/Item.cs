using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ListFoodWebApp.Models
{
    public class Item
    {
        [Required]
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
        public virtual FoodCategory FoodCategory { get; set; }
    }
}

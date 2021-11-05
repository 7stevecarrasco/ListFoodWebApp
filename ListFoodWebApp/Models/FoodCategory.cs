using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ListFoodWebApp.Models
{
    public class FoodCategory
    {
        public Guid FoodCategoryID { get; set; }
        [Display(Name = "Parent Category")]
        [ForeignKey(nameof(FoodCategoryID))]
        public Guid? ParentCategoryID { get; set; }
        [Display(Name = "Category Name")]
        public string Name { get; set; }
        public Boolean Active { get; set; }
        public ICollection<Item> Items { get; set; }
        public string FoodCategoryImageName { get; set; }
        public virtual FoodCategory ParentCategory { get; set; }
        public virtual ICollection<FoodCategory> Children { get; set; } = new List<FoodCategory>();

    }
}

using ListFoodWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ListFoodWebApp.ViewModels
{
    public class ProductViewModel
    {
        public ProductViewModel()
        {
            ProductSubCategories = new List<ProductSubCategory>();
        }
        public Guid ProductParentId { get; set; }
        public string ProductParentName { get; set; }
        public bool ProductParentActive { get; set; }
        public string ProductParentImageName { get; set; }
        public List<ProductSubCategory> ProductSubCategories { get; set; }
    }

    public class ProductSubCategory
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public Boolean ProductActive { get; set; }
        public string FoodCategoryImageName { get; set; }
        public virtual ICollection<FoodCategory> Children { get; set; } = new List<FoodCategory>();
    }
}

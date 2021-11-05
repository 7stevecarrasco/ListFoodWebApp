using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ListFoodWebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
          : base(options)
        {
        }
        public DbSet<ListFoodWebApp.Models.ViewItem> ViewItems { get; set; }
        public DbSet<ListFoodWebApp.Models.FoodCategory> FoodCategories { get; set; }
        public DbSet<ListFoodWebApp.Models.Item> Item { get; set; }

        
        public DbSet<ListFoodWebApp.Models.Order> Orders { get; set; }
       
        public DbSet<ListFoodWebApp.Models.PurchaseItem> PurchaseItems { get; set; }
       
        public DbSet<ListFoodWebApp.Models.PurchasedList> PurchasedList { get; set; }
        public DbSet<ListFoodWebApp.Models.OrderedItem> OrderedItems { get; set; }
        public DbSet<ListFoodWebApp.Models.Event> Events { get; set; }





    }
}

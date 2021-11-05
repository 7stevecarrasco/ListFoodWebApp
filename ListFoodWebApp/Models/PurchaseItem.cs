using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ListFoodWebApp.Models
{
    public class PurchaseItem
    {
        public Guid PurchaseItemID { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public double Cost { get; set; }
        public Guid ItemID { get; set; }
    }
}

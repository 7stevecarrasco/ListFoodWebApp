using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ListFoodWebApp.Models
{
    public class PurchasedList
    {
        [Key]
        public Guid PurchasedListID { get; set; }
        public Guid OrderedItemID { get; set; }
        public Guid PurchasedItemID { get; set; }
        public int Quantity { get; set; }
        public virtual PurchaseItem PurchaseItem { get; set; }
        public virtual OrderedItem OrderedItem { get; set; }
        public Guid PurchaseItemID { get; internal set; }
    }
}

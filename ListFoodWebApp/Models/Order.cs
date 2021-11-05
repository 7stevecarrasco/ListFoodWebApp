using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ListFoodWebApp.Models
{
    public class Order
    {
        public enum StatusType
        {
            FinishedList, ListSubmitted, ListCancelled
        }
        [Key]
        public Guid OrderID { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public Guid EventID { get; set; }
        [DisplayFormat(DataFormatString = "{0:dddd MM/dd/yy hh:mm tt}")]
        public DateTime DateOrdered { get; set; }
        [DisplayFormat(DataFormatString = "{0:dddd MM/dd/yy hh:mm tt}")]
        public DateTime PickupDate { get; set; }
        public StatusType? Status { get; set; }
        public Boolean Generic { get; set; }
        public virtual Event Event { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ListFoodWebApp.Models.ViewModels
{
    public class OrderStatusVM
    {
        public enum StatusType
        {
            FinishedList, ListSubmitted, ListCancelled
        }
        public Guid OrderID { get; set; }
        public StatusType? Status { get; set; }
    }
}
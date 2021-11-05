using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ListFoodWebApp.Models
{
    public class Event
    {
        public Guid EventId { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Description { get; set; }
        public string EventType { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int Zipcode { get; set; }
        public string CreatedBy { get; set; }
        //public string UserId { get; set; }
        //public virtual ApplicationUser User { get; set; }

    }
}

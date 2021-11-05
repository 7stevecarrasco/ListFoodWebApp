using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ListFoodWebApp.Models
{
    public class ViewItem
    {
        [Key]
        public Guid itemId { get; set; }
        public string itemName { get; set; }

    }
}

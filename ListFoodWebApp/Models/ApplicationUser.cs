using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ListFoodWebApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<UserInfo> UserInfos { get; set; }
    }
}

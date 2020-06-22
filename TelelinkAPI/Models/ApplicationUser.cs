using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TelelinkAPI.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
       
        public Owner Owner { get; set; }
     

    }
}

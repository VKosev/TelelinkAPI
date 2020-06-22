using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelelinkAPI.Models;

namespace TelelinkAPI.POCOs
{
    public class POCOUser : ApplicationUser
    {
        public string Password { get; set; }
    }
}

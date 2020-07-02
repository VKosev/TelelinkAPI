using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelelinkAPI.Models;

namespace TelelinkAPI.POCOs
{
    public class RoleAndPasswordUserPoco : ApplicationUser
    {
        public string Password { get; set; }
        public string Role { get; set; }
    }
}

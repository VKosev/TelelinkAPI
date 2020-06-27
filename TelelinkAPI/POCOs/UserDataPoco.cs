using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelelinkAPI.POCOs
{
    public class UserDataPoco
    {
        public string Username { get; set; }
        public string OwnerName { get; set; }
        public string Email { get; set; }
        public List<string> ModelsNames { get; set; }


        public UserDataPoco()
        {
            ModelsNames = new List<String>();
        }

    }

    
}

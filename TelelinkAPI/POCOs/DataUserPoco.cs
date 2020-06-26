using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelelinkAPI.POCOs
{
    public class DataUserPoco
    {
        public string Username { get; set; }
        public string OwnerName { get; set; }
        public List<string> ModelsNames { get; set; }

    }
}

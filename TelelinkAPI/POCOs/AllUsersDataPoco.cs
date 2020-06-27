using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelelinkAPI.Models;

namespace TelelinkAPI.POCOs
{
    public class AllUsersDataPoco
    {
        public string OwnerName { get; set; }
        public string ModelName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public OwnerModel OwnerModels { get; set; }

        public AllUsersDataPoco()
        {
            //OwnerModels = new List<OwnerModel>();
        }
    }
}

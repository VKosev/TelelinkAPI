using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelelinkAPI.POCOs
{
    // Poco used to get info about model and how many times specific owner have this model
    public class ModelAddsToOwnerCountPoco
    {
        public int ModelId { get; set; }
        public string ModelName { get; set; }
        public int AddsToOwner { get; set; } // Shows count, how many times model is added to owner.
    }
}

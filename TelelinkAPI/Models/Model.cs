using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TelelinkAPI.Models
{
    public class Model
    {
        public int Id { get; set; }

        [Required]
        [StringLength(450)]
        public string Name { get; set; }

        public ICollection<OwnerModel> OwnerModels { get; set; }

        
    }
}

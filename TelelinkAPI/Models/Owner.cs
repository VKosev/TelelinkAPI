using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TelelinkAPI.Models
{
    public class Owner
    {
        public int Id { get; set; }

        [Required]
        [StringLength(450)]
        public string Name { get; set; }

        [Required]
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }

        public ICollection<OwnerModel> OwnerModels { get; set; }
    }
}
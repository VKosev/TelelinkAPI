using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
namespace TelelinkAPI.Models
{
    public class OwnerModel
    {
        public int Id { get; set; }

        public int OwnerId { get; set; }
        public Owner Owner { get; set; }

        public int ModelId { get; set; }
        public Model Model { get; set; }

        [Required]
        public DateTime Date { get; set; }
        public String Description { get; set; }
    }
}
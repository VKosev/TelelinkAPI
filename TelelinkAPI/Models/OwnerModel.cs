using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
namespace TelelinkAPI.Models
{
    public class OwnerModel
    {  
        public int OwnerId { get; set; }
        public Owner Owner { get; set; }

        public int ModelId { get; set; }
        public Model Model { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public String Description { get; set; }
    }
}
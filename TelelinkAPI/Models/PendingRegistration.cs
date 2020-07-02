using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TelelinkAPI.Models
{
    public class PendingRegistration
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }

        [Column(TypeName = "varbinary(800)")]
        public Byte[] EncriptedPassword { get; set; }
        public string Email { get; set; }
        public string OwnerName { get; set; }
        public string Role { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class MenuAccess
    {
        [Key, Column(Order = 1)]
        public int MenuId { get; set; }
        [Key, Column(Order = 2)]
        public int RoleId { get; set; }
        [ForeignKey("MenuId")]
        public Menu Menu { get; set; }
        [ForeignKey("RoleId")]
        public Role Role { get; set; }        
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Menu
    {
        public int MenuId { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public string URL { get; set; }
        public int? ParentMenuId { get; set; }        
        [ForeignKey("ParentMenuId")]
        public virtual Menu ParentMenu { get; set; }        
        public virtual ICollection<Menu> Submenus { get; set; }
        public ICollection<MenuAccess> MenuAccesses { get; set; }
    }
}

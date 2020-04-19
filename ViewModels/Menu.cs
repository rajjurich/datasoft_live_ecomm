using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class Menu
    {
        public int MenuId { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public string URL { get; set; }
        public int? ParentMenuId { get; set; }       
        public virtual Menu ParentMenu { get; set; }
        public virtual List<Menu> Submenus { get; set; }
        public List<MenuAccess> MenuAccesses { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos
{
    public class MenuDto
    {
        public int MenuId { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public string URL { get; set; }
        public ICollection<MenuDto> Submenus { get; set; }
    }
}

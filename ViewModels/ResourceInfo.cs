using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class ResourceInfo
    {
        public int ResourceId { get; set; }
        [Display(Name = "Resource")]
        public string ResourceName { get; set; }
        [Display(Name = "Company")]
        public string CompanyName { get; set; }
        public int? CompanyId { get; set; }
        [Display(Name = "Role")]
        public string Role { get; set; }
        public int? RoleId { get; set; }
    }
}

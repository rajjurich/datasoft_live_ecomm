using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos
{
    public class ResourceInfoDto
    {
        public int ResourceId { get; set; }
        public string ResourceName { get; set; }
        public int? CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int RoleId { get; set; }
        public string Role { get; set; }
    }
}

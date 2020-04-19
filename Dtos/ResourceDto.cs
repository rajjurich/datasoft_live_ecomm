using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos
{
    public class ResourceDto
    {
        public int ResourceId { get; set; }
        public string ResourceName { get; set; }
        public int CompanyId { get; set; }
        public int RoleId { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
        public string RoleName { get; set; }
    }
}

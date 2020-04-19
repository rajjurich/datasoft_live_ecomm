using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class Login
    {
        [Required(ErrorMessage = "required")]
        public string username { get; set; }
        [Required(ErrorMessage = "required")]
        public string password { get; set; }
        public string grant_type { get; set; }
    }
}

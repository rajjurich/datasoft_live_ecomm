using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ApplicationConfiguration
    {
        public Guid ApplicationConfigurationId { get; set; }
        public string ApplicationOwnerName { get; set; }
        public string ApplicationVersion { get; set; }
        public string SystemInformation { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}

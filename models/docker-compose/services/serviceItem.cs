using System.Collections.Generic;

namespace miccore.Models
{
    class ServiceItem
    {
        public string container_name {get; set;}
        public List<string> depends_on {get; set;}
        public List<string> expose {get; set;}
        public List<string> ports {get; set;}
        public string image {get; set;}
        public Environment environment {get; set;}
        public Network networks {get; set;}
    }
}
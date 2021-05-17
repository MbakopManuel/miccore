using System.Collections.Generic;

namespace miccore.Models
{
    class Package
    {
        public string Name {get; set;}
        public string Version {get; set;}
        public List<Project> Projects {get; set;}
    }
}
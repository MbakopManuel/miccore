using System.Collections.Generic;

namespace miccore.Models
{
    class Project
    {
        public string Name {get; set;}
        public string Port {get; set;}
        public string DockerUrl {get; set;}
        public List<string> references {get; set;}
        public List<string> Services {get; set;}
    }
}
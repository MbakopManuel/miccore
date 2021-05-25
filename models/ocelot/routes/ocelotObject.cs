using System.Collections.Generic;

namespace miccore.Models
{
    class OcelotObject
    {
        public string DownstreamPathTemplate {get; set;}
        public string DownstreamScheme {get; set;}
        public List<DownstreamHostAndPort> DownstreamHostAndPorts {get; set;}
        public string UpstreamPathTemplate {get; set;}
        public List<string> UpstreamHttpMethod {get; set;}
        public string SwaggerKey {get; set;}

    }
}
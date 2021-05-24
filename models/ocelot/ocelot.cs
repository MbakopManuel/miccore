using System.Collections.Generic;

namespace miccore.Models
{
    class Ocelot
    {
        public List<OcelotObject> Routes {get; set;}
        public List<SwaggerEndPointObject> SwaggerEndPoints {get; set;}

    }
}
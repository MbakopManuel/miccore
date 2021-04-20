using McMaster.Extensions.CommandLineUtils;
using miccore.project;
using miccore.service;
using Microsoft.Extensions.Logging;

namespace miccore
{
    [Command(Name = "new", Description = "new Web Microservice Api architecture with ocelot gateway")]
    class newCmd : miccoreBaseCmd
    {
        
        public newCmd(ILogger<newCmd> logger, IConsole console){
            _logger = logger;
            _console = console;
        }

    }
}
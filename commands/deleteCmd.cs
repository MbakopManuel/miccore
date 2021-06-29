using McMaster.Extensions.CommandLineUtils;
using miccore.project;
using miccore.service;
using Microsoft.Extensions.Logging;

namespace miccore
{
    [Command(Name = "delete", Description = "delete micro-project in our architecture or a service in one existing micro-project")]
    [Subcommand( typeof(deleteProjectCmd),
                 typeof(serviceCmd))]
    class deleteCmd : miccoreBaseCmd
    {
        
        public deleteCmd(ILogger<deleteCmd> logger, IConsole console){
            _logger = logger;
            _console = console;
        }

    }
}
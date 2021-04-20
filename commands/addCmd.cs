using McMaster.Extensions.CommandLineUtils;
using miccore.project;
using miccore.service;
using Microsoft.Extensions.Logging;

namespace miccore
{
    [Command(Name = "add", Description = "add micro-project in our architecture or add a service in one existing micro-project ")]
    [Subcommand( typeof(projectCmd),
                 typeof(serviceCmd))]
    class addCmd : miccoreBaseCmd
    {
        
        public addCmd(ILogger<addCmd> logger, IConsole console){
            _logger = logger;
            _console = console;
        }

    }
}
using McMaster.Extensions.CommandLineUtils;
using miccore.project;
using miccore.service;
using miccore.reference;
using Microsoft.Extensions.Logging;

namespace miccore
{
    [Command(Name = "add", Description = "add micro-project in our architecture or add a service in one existing micro-project> you can also reference a project ")]
    [Subcommand( typeof(referenceCmd),
                 typeof(projectCmd),
                 typeof(serviceCmd))]
    class addCmd : miccoreBaseCmd
    {
        
        public addCmd(ILogger<addCmd> logger, IConsole console){
            _logger = logger;
            _console = console;
        }

    }
}
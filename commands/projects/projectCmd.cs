using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace miccore.project
{
    [Command(Name = "project", Description = "manage projects")]
    class projectCmd : miccoreBaseCmd
    {
        
        public projectCmd(ILogger<projectCmd> logger, IConsole console){
            _logger = logger;
            _console = console;
        }

    }
}
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace miccore.service
{
    [Command(Name = "service", Description = "manage services")]
    class serviceCmd : miccoreBaseCmd
    {
        
        public serviceCmd(ILogger<serviceCmd> logger, IConsole console){
            _logger = logger;
            _console = console;
        }

    }
}
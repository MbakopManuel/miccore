using System;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace miccore.service
{
    [Command(Name = "service", Description = "manage services")]
    class serviceCmd : miccoreBaseCmd
    {
         [Option("--project | -p",
                Description = "the project name in which you want the service to be created",
                ShowInHelpText = true
                )]
        public bool _auth {get;}

        [Option(" --name | -n",
                CommandOptionType.SingleValue,
                Description = "The name of the service",
                ShowInHelpText = true)]
        public string _name {get; set; }
        public serviceCmd(ILogger<serviceCmd> logger, IConsole console){
            _logger = logger;
            _console = console;
        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            try
            {
                
                return 0;
            }
            catch (Exception ex)
            {
                OnException(ex);
                return 1;
            }
        }

    }
}
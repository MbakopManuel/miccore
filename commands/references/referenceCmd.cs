using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using miccore.Utility;
using Microsoft.Extensions.Logging;

namespace miccore.reference
{
    [Command(Name = "reference", Description = "manage reference from another project to another one")]
    class referenceCmd : miccoreBaseCmd
    {
         [Option("--from | -f",
                Description = "project name of the project which will be refered",
                ShowInHelpText = true
                )]
        public bool _from {get;}

        [Option(" --to | -t",
                CommandOptionType.SingleValue,
                Description = "The name of the project where the reference will be applied",
                ShowInHelpText = true)]
        public string _to {get; set; }
        public referenceCmd(ILogger<referenceCmd> logger, IConsole console){
            _logger = logger;
            _console = console;
        }

        protected override Task<int> OnExecute(CommandLineApplication app)
        {
            try
            {
               
               
                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                OnException(ex);
                return Task.FromResult(1);
            }
        }

    }
}
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace miccore.project
{
    [Command(Name = "project", Description = "manage projects")]
    class projectCmd : miccoreBaseCmd
    {
         [Option("--with-auth | -wa",
                Description = "create a project with authentication or not, if specified your project will be created with an authentication microservice and otherwise it will be created with a sample microservice called sample",
                ShowInHelpText = true
                )]
        public bool _auth {get;}

        [Option(" --name | -n",
                CommandOptionType.SingleValue,
                Description = "The name of the project",
                ShowInHelpText = true)]
        public string _name {get; set; }
        public projectCmd(ILogger<projectCmd> logger, IConsole console){
            _logger = logger;
            _console = console;
        }

    }
}
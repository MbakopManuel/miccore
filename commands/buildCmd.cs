using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using miccore.Utility;
using Microsoft.Extensions.Logging;

namespace miccore
{
    [Command(Name = "build", Description = "build the project and get the dist folder which have the dll files and the sh file for serving the webapi")]
    class buildCmd : miccoreBaseCmd
    {

        [Option("--open | -o",
                Description = "if mentionned, serve the builder folder and launch the app",
                ShowInHelpText = true
                )]
        public bool _open {get;}

        public buildCmd(ILogger<newCmd> logger, IConsole console){
            _logger = logger;
            _console = console;
        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            try
            {
                if(File.Exists("./Microservice.WebApi.sln")){
                    
                     if(!File.Exists("./package.json")){
                         OutputError("breaking project, package.json file doesn't exist\n");
                     }

                     InjectionUtility injection = new InjectionUtility();
                     injection.SHFilesCreationAndInject("./package.json");

                     if(_open){
                        Directory.SetCurrentDirectory("./dist");
                        var process1 = Process.Start("./start.sh");
                        process1.WaitForExit();
                     }
                   
                }else{
                    OutputError("microservice solution not found.\ngo to the general project");
                }
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
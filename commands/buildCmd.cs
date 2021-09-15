using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using miccore.Utility;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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

        [Option("--docker | -d",
                Description = "if mentionned, serve the builder folder and launch the app",
                ShowInHelpText = true
                )]
        public bool _docker {get;}

        public buildCmd(ILogger<newCmd> logger, IConsole console){
            _logger = logger;
            _console = console;
        }

        protected override Task<int> OnExecute(CommandLineApplication app)
        {
            try
            {
                if(File.Exists("./Microservice.WebApi.sln")){
                    
                    if(!File.Exists("./package.json")){
                        OutputError("breaking project, package.json file doesn't exist\n");
                        return Task.FromResult(1);
                    }

                   

                    if(!_docker){
                        InjectionUtility injection = new InjectionUtility();
                        injection.SHFilesCreationAndInject("./package.json");
                        
                        bool ch = Chmod("./start.sh", "777");
                        // if(_open){
                        //     var process1 = Process.Start(Path.GetFullPath("start.sh"));
                        //     process1.WaitForExit();
                        // }
                    }else{

                        InjectionUtility injection = new InjectionUtility();
                        injection.DockerFilesCreationAndInject("./package.json");
                        

                    }

                }else{
                    OutputError("microservice solution not found.\ngo to the general project");
                    return Task.FromResult(1);
                }
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
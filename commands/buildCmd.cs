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
                // check if it is microservice webapi solution
                if(!File.Exists("./Microservice.WebApi.sln")){
                    // if not return error
                    OutputError("microservice solution not found.\ngo to the general project");
                    return Task.FromResult(1);
                }
                
                // check if package json file exist
                if(!File.Exists("./package.json")){
                    // if not return error
                    OutputError("breaking project, package.json file doesn't exist\n");
                    return Task.FromResult(1);
                }

                // check if it's a build for docker
                if(!_docker){
                    // if it's build, create sh files
                    InjectionUtility injection = new InjectionUtility();
                    injection.SHFilesCreationAndInject("./package.json");
                    // add execution mode to file generated
                    bool ch = Chmod("./start.sh", "777");

                    return Task.FromResult(0);
                }

                // create dockerfiles for building
                InjectionUtility injection = new InjectionUtility();
                injection.DockerFilesCreationAndInject("./package.json");
                
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
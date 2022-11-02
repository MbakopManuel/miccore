using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using miccore.Models;
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

        // [Option("--docker | -d",
        //         Description = "if mentionned, serve the builder folder and launch the app",
        //         ShowInHelpText = true
        //         )]
        // public bool _docker {get;}

        [Option("--project | -p",
                Description = "if mentionned, build only one project",
                ShowInHelpText = true
                )]
        public string _project {get;}

        public buildCmd(ILogger<buildCmd> logger, IConsole console){
            _logger = logger;
            _console = console;
        }

        protected override Task<int> OnExecute(CommandLineApplication app)
        {
            var process = new Process();
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            try
            {
                 // check if package json file exist
                if(!File.Exists("./package.json")){
                    OutputError("Error: Package file not found");
                    return Task.FromResult(1);
                }

                // get the company name to the package json file
                var text = File.ReadAllText("./package.json");
                Package package = JsonConvert.DeserializeObject<Package>(text);
               
                // company name
                string companyName = package.Company;
                string projectName = package.Name;
                // check if it's microservice webapi solution
                if(!File.Exists($"./{companyName}.{projectName}.sln")){
                    // return error if not
                    OutputError("Microservice solution not found, go to the general project");
                    return Task.FromResult(1);
                }

                if(!string.IsNullOrEmpty(_project)){
                    var project = package.Projects.Find(x => x.Name == _project);

                    if(project is null){
                        OutputError("Error: Project not found");
                        return Task.FromResult(1);
                    }

                    var name = $"{companyName}.{projectName}.{_project}";
                    // restore
                    restoreProject(name, process);
                    
                    // publish
                    publishProject(name, process);

                     // build image
                    buildImage(name, process);
                    
                    // save image in tar file
                    saveImage(name, process);

                    
                    return Task.FromResult(0);
                }
                
                InjectionUtility injection = new InjectionUtility(_logger);

                // // create dockerfiles for building
                injection.DockerFilesCreationAndInject("./package.json", "build");
                
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
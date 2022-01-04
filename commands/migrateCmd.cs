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
    [Command(Name = "migrate", Description = "migrate or update database with all migration depending of the project references")]
    class migrateCmd : miccoreBaseCmd
    {


        [Option("--dotnet_exec_path | -d",
                Description = "if mentionned, serve the builder folder and launch the app",
                ShowInHelpText = true
                )]
        public string _dotnet {get;}
        
        public migrateCmd(ILogger<newCmd> logger, IConsole console){
            _logger = logger;
            _console = console;
        }

        protected override Task<int> OnExecute(CommandLineApplication app)
        {

            // Run all migrations 
            try
            {
                // check if it's a dotnet core solution
                if(!File.Exists("./Microservice.WebApi.sln")){
                    // if it's not a dotnet core solution, return error
                    OutputError("microservice solution not found.\ngo to the general project");
                    return Task.FromResult(1);
                }

                // check if package.json exist in the project
                if(!File.Exists("./package.json")){
                    // if not return error
                    OutputError("breaking project, package.json file doesn't exist\n");
                    return Task.FromResult(1);
                }

                // dependency tree variable
                List<string> schedule = new List<string>();

                // read the package json file
                var text = File.ReadAllText("./package.json");
                Package package = JsonConvert.DeserializeObject<Package>(text);

                // create dependency tree
                OutputToConsole($" \n******************************************************************************************** \n");
                OutputToConsole($"   Dependency tree creation ... \n");
                OutputToConsole($" \n******************************************************************************************** \n\n");
                var projets = package.Projects;
                projets.ForEach(projet => {
                    var name = projet.Name.Split(".")[0].ToLower();
                    // check all element without gateway
                    if(name != "gateway"){
                        // if project doesn't have references, we insert it at index 0
                        if(projet.references.Count == 0){
                            // check if it isn't exist yet
                            if(!schedule.Contains(name)){
                                schedule.Insert(0, name);
                            }
                        }
                        // if project has  references, add it to the tree
                        if(projet.references.Count != 0){
                            // foreach references add to the list
                            projet.references.ForEach( refs => {
                                // check if it isn't exist yet
                                if(!schedule.Contains(refs)){
                                    schedule.Add(refs);
                                }
                            });
                            // check if it isn't exist yet, and add project
                            if(!schedule.Contains(name)){
                                schedule.Add(name);
                            }
                        }
                    }
                });

                // get full path of the solution
                var current = Path.GetFullPath(".");

                // get dotnet ef command, if it's passed in parameter or is installed globally
                var exec = (string.IsNullOrEmpty(_dotnet)) ? "dotnet-ef" :  _dotnet;
                // foreach project in the dependency tree, run migrations
                schedule.ForEach( x => {
                    // name of the project
                    var name = $"{x[0].ToString().ToUpper()}{x.Substring(1)}.Api";

                    OutputToConsole($" \n******************************************************************************************** \n");
                    OutputToConsole($"   {name} migration ... \n");
                    OutputToConsole($" \n******************************************************************************************** \n\n");
                    // set current directory to project directory
                    Directory.SetCurrentDirectory($"{current}/{name}/{name}");
                    // run the dotnet ef databse update command
                    var process = Process.Start(exec, "database update");
                    process.WaitForExit();
                });

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
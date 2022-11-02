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

        [Option("--project | -p",
                Description = "if mentionned, migration of only one project",
                ShowInHelpText = true
                )]
        public string _project {get;}
        
        public migrateCmd(ILogger<migrateCmd> logger, IConsole console){
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
            // Run all migrations 
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
                    OutputError("Error: Microservice solution not found, go to the general project");
                    return Task.FromResult(1);
                }

                
                OutputToConsole($"Build the solution");
                process.StartInfo.FileName = "dotnet";
                process.StartInfo.Arguments = $"build";
                process.Start();
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    OutputError(process.StandardError.ReadToEnd());
                    throw new Exception(process.StandardError.ReadToEnd());
                }

                // get dotnet ef command, if it's passed in parameter or is installed globally
                var exec = (string.IsNullOrEmpty(_dotnet)) ? "dotnet-ef" :  _dotnet;

                if(string.IsNullOrEmpty(_project)){
                    var project = package.Projects.Find(x => x.Name == _project);
                    if(project is null){
                        OutputError("Error: Project not found");
                        return Task.FromResult(1);
                    }

                    var name = $"{companyName}.{projectName}.{_project}";
                    OutputToConsole($"{name} migration ... ");
                    // set current directory to project directory
                    Directory.SetCurrentDirectory($"./{name}/src/{name}.Api");
                    process.StartInfo.FileName = exec;
                    process.StartInfo.Arguments = $"databse update";
                    process.Start();
                    process.WaitForExit();
                    if (process.ExitCode != 0)
                    {
                        OutputError(process.StandardError.ReadToEnd());
                        throw new Exception(process.StandardError.ReadToEnd());
                    }
                    return Task.FromResult(0);
                }



                // dependency tree variable
                List<string> schedule = new List<string>();

                // create dependency tree
                OutputToConsole($"Dependency tree creation ... ");
                
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

                // foreach project in the dependency tree, run migrations
                schedule.ForEach( x => {
                    // name of the project
                    var name = $"{package.Company}.{package.Name}.{x}";
                    OutputToConsole($"{name} migration ... ");
                    // set current directory to project directory
                    Directory.SetCurrentDirectory($"{current}/{name}/src/{name}.Api");
                    // run the dotnet ef databse update command
                    process.StartInfo.FileName = exec;
                    process.StartInfo.Arguments = $"databse update";
                    process.Start();
                    process.WaitForExit();
                    if (process.ExitCode != 0)
                    {
                        OutputError(process.StandardError.ReadToEnd());
                        throw new Exception(process.StandardError.ReadToEnd());
                    }
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
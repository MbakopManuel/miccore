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
            try
            {
                if(File.Exists("./Microservice.WebApi.sln")){
                    
                    if(!File.Exists("./package.json")){
                        OutputError("breaking project, package.json file doesn't exist\n");
                        return Task.FromResult(1);
                    }

                    List<string> schedule = new List<string>();

                    var text = File.ReadAllText("./package.json");
                    Package package = JsonConvert.DeserializeObject<Package>(text);

                    OutputToConsole($" \n******************************************************************************************** \n");
                    OutputToConsole($"   Dependency tree creation ... \n");
                    OutputToConsole($" \n******************************************************************************************** \n\n");

                    var projets = package.Projects;
                    projets.ForEach(projet => {
                        var name = projet.Name.Split(".")[0].ToLower();
                        if(name != "gateway"){
                            if(projet.references.Count == 0){
                                if(!schedule.Contains(name)){

                                    schedule.Insert(0, name);
                                }
                            }else{
                                projet.references.ForEach( refs => {
                                    if(!schedule.Contains(refs)){
                                        schedule.Add(refs);
                                    }
                                });
                                if(!schedule.Contains(name)){
                                    schedule.Add(name);
                                }
                            }
                        }
                    });

                    // schedule.ForEach(x => Console.WriteLine(x));

                    var current = Path.GetFullPath(".");
                    var exec = "";
                    if(string.IsNullOrEmpty(_dotnet)){
                        exec = "dotnet-ef";
                    }else{
                        exec = _dotnet;
                    }
                    schedule.ForEach( x => {
                        var name = $"{x[0].ToString().ToUpper()}{x.Substring(1)}.Microservice";

                        OutputToConsole($" \n******************************************************************************************** \n");
                        OutputToConsole($"   {name} migration ... \n");
                        OutputToConsole($" \n******************************************************************************************** \n\n");

                        Directory.SetCurrentDirectory($"{current}/{name}/{name}");
                        
                        var process = Process.Start(exec, "database update");
                        process.WaitForExit();
                        if(process.ExitCode != 0){
                            throw new Exception(process.StandardError.ReadLine());
                        }
                    });

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
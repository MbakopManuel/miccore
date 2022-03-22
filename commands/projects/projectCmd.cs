using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using miccore.Utility;
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

        protected override Task<int> OnExecute(CommandLineApplication app)
        {
            try
            {
                if(File.Exists("./Microservice.WebApi.sln")){
                    
                    if(string.IsNullOrEmpty(_name)){
                        OutputError($"\n name option is required to execute this command\n\n");
                        return Task.FromResult(1);
                    }

                    _name = char.ToUpper(_name[0]) + _name.Substring(1).ToLower();
                    var name = _name;
                    _name = $"{_name}.Microservice";
                    
                    if(Directory.Exists($"./{_name}")){
                        OutputError($"\nProject {_name} already exist, please create another or change the name\n\n");
                        return Task.FromResult(1);
                    }
                    
                    if(_auth){
                        
                        OutputToConsole($" \n******************************************************************************************** \n");
                        OutputToConsole($"   add microservice with authentication with name {_name} ... \n");
                        OutputToConsole($" \n******************************************************************************************** \n\n");
                        runCloneProject(_name, _source_user_microservice_net6);

                        OutputToConsole($" \n******************************************************************************************** \n");
                        OutputToConsole($"  renaming ... \n");
                        OutputToConsole($" \n******************************************************************************************** \n\n");
                        Directory.SetCurrentDirectory($"../");
                        RenameUtility rename = new RenameUtility();
                        rename.Rename($"{_name}/", "User.Microservice", _name);
                        rename.Rename($"{_name}/", "User.Test.Microservice", $"{name}.Test.Microservice/");

                    }else{
                        
                        OutputToConsole($" \n******************************************************************************************** \n");
                        OutputToConsole($"   add microservice without authentication with name {_name} ... \n");
                        OutputToConsole($" \n******************************************************************************************** \n\n");
                        runCloneProject(_name, _source_sample_microservice_net6); 
                        

                        OutputToConsole($" \n******************************************************************************************** \n");
                        OutputToConsole($"  renaming ... \n");
                        OutputToConsole($" \n******************************************************************************************** \n\n");
                        Directory.SetCurrentDirectory($"../");
                        RenameUtility rename = new RenameUtility();
                        rename.Rename($"./{_name}/", "Sample", name);
                        rename.Rename($"./{_name}/", "sample",  char.ToLower(name[0]) + name.Substring(1).ToLower());

                    }
                   
                    OutputToConsole($" \n******************************************************************************************** \n");
                    OutputToConsole($"  adding project to the solution ... \n");
                    OutputToConsole($" \n******************************************************************************************** \n\n");
                    var process = Process.Start("dotnet", $"sln Microservice.WebApi.sln add ./{_name}/{_name}/{_name}.csproj");
                    process.WaitForExit();
                    process = Process.Start("dotnet", $"sln Microservice.WebApi.sln add ./{_name}/{name}.Test.Microservice/{name}.Test.Microservice.csproj");
                    process.WaitForExit();

                    OutputToConsole($" \n******************************************************************************************** \n");
                    OutputToConsole($"  build the solution ... \n");
                    OutputToConsole($" \n******************************************************************************************** \n\n");
                    process = Process.Start("dotnet", $"build");
                    process.WaitForExit();

                    OutputToConsole($" \n******************************************************************************************** \n");
                    OutputToConsole($"  package json injection ... \n");
                    OutputToConsole($" \n******************************************************************************************** \n\n");
                    InjectionUtility injection = new InjectionUtility();
                    injection.PackageJsonProjectInject("./package.json", _name, _auth);
                    
                    OutputToConsole($" \n******************************************************************************************** \n");
                    OutputToConsole($"  Ocelot json injection ... \n");
                    OutputToConsole($" \n******************************************************************************************** \n\n");
                    injection.OcelotProjectInjection("./Gateway.WebApi/ocelot.json", name);

                    OutputToConsole($" \n******************************************************************************************** \n");
                    OutputToConsole($"  Docker compose json injection ... \n");
                    OutputToConsole($" \n******************************************************************************************** \n\n");
                    injection.DockerProjectInjection("./docker-compose.yml", name);

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
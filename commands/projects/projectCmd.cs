using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using miccore.Utility;
using miccore.Models;
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
            RenameUtility rename = new RenameUtility();
            InjectionUtility injection = new InjectionUtility();
            try
            {
                
                // check if it's microservice webapi solution
                if(!File.Exists("./Microservice.WebApi.sln")){
                    // return error if not
                     OutputError("microservice solution not found.\ngo to the general project");
                    return Task.FromResult(1);
                }
                
                // check if name is given
                if(string.IsNullOrEmpty(_name)){
                    // return error if not
                    OutputError($"\n name option is required to execute this command\n\n");
                    return Task.FromResult(1);
                }

                // check if package json file exist
                if(!File.Exists(filepath)){
                    OutputError("\n\nError: Package file not found\n\n");
                    return;
                }
                // get the company name to the package json file
                var text = File.ReadAllText(filepath);
                Package package = JsonConvert.DeserializeObject<Package>(text);
                // company name
                string companyName = package.CompanyName;
                string projectName = package.Name;

                // parsethe name to camel case
                _name = char.ToUpper(_name[0]) + _name.Substring(1).ToLower();
                var name = _name;
                _name = $"{_name}.Api";
                
                // check if project with that name exist already
                if(Directory.Exists($"./{_name}")){
                    // return error if it's
                    OutputError($"\nProject {_name} already exist, please create another or change the name\n\n");
                    return Task.FromResult(1);
                }
                
                // if it's project with authentification 
                if(_auth){
                    
                    // clone the project
                    OutputToConsole($" \n******************************************************************************************** \n");
                    OutputToConsole($"   add microservice with authentication with name {_name} ... \n");
                    OutputToConsole($" \n******************************************************************************************** \n\n");
                    runCloneProject(_name, _source_user_microservice);

                    // rename elements 
                    OutputToConsole($" \n******************************************************************************************** \n");
                    OutputToConsole($"  renaming ... \n");
                    OutputToConsole($" \n******************************************************************************************** \n\n");
                    Directory.SetCurrentDirectory($"../");
                    
                    // rename user by the name of project
                    rename.Rename($"{_name}/", "User.Api", _name);
                    rename.Rename($"{_name}/", "User.Api.Test", $"{_name}.Test/");

                }
                // if it's without authentification
                else{
                    
                    // clone the project
                    OutputToConsole($" \n******************************************************************************************** \n");
                    OutputToConsole($"   add microservice without authentication with name {_name} ... \n");
                    OutputToConsole($" \n******************************************************************************************** \n\n");
                    runCloneProject(_name, _source_sample_microservice); 
                    

                    // rename the elements
                    OutputToConsole($" \n******************************************************************************************** \n");
                    OutputToConsole($"  renaming ... \n");
                    OutputToConsole($" \n******************************************************************************************** \n\n");
                    Directory.SetCurrentDirectory($"../");
                    
                    rename.Rename($"./{_name}/", "Sample", name);
                    rename.Rename($"./{_name}/", "sample",  char.ToLower(name[0]) + name.Substring(1).ToLower());

                }

                // rename company name by default for the real company name
                rename.Rename($"{_name}/", "Miccore.Net", companyName);
                // rename project name by default for the real project name
                rename.Rename($"{_name}/", "webapi_template", projectName);
                
                // adding the project to the existing solution
                OutputToConsole($" \n******************************************************************************************** \n");
                OutputToConsole($"  adding project to the solution ... \n");
                OutputToConsole($" \n******************************************************************************************** \n\n");
                // add api project ot solution
                var process = Process.Start("dotnet", $"sln Microservice.WebApi.sln add ./{_name}/{_name}/{_name}.csproj");
                process.WaitForExit();
                // add api test project to solution
                process = Process.Start("dotnet", $"sln Microservice.WebApi.sln add ./{_name}/{_name}.Test/{_name}.Test.csproj");
                process.WaitForExit();

                // buil the solution
                OutputToConsole($" \n******************************************************************************************** \n");
                OutputToConsole($"  build the solution ... \n");
                OutputToConsole($" \n******************************************************************************************** \n\n");
                process = Process.Start("dotnet", $"build");
                process.WaitForExit();

                // inject the project to the package json file
                OutputToConsole($" \n******************************************************************************************** \n");
                OutputToConsole($"  package json injection ... \n");
                OutputToConsole($" \n******************************************************************************************** \n\n");
                injection.PackageJsonProjectInject("./package.json", _name, _auth);
                
                // inject the project to the ocelot file
                OutputToConsole($" \n******************************************************************************************** \n");
                OutputToConsole($"  Ocelot json injection ... \n");
                OutputToConsole($" \n******************************************************************************************** \n\n");
                injection.OcelotProjectInjection("./Gateway.WebApi/ocelot.json", name);

                // inject the project to the docker compose file
                OutputToConsole($" \n******************************************************************************************** \n");
                OutputToConsole($"  Docker compose json injection ... \n");
                OutputToConsole($" \n******************************************************************************************** \n\n");
                injection.DockerProjectInjection("./docker-compose.yml", name);

               
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
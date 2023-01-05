using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using miccore.Utility;
using miccore.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
            InjectionUtility injection = new InjectionUtility(_logger);
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
                     OutputError("Microservice solution not found.\ngo to the general project");
                    return Task.FromResult(1);
                }
                
                // check if name is given
                if(string.IsNullOrEmpty(_name)){
                    // return error if not
                    OutputError($"Name option is required to execute this command");
                    return Task.FromResult(1);
                }

                // check if project already exist
                var project = package.Projects.Find(x => x.Name == _name);
                if(project is not null){
                    OutputError($"Error: Project {_name}  already exist, please create another or change the name");
                    return Task.FromResult(1);
                }

                // parse the name to camel case
                _name = char.ToUpper(_name[0]) + _name.Substring(1).ToLower();
                var name = _name;
                _name = $"{companyName}.{projectName}.{_name}";
                
                // check if project with that name exist already
                if(Directory.Exists($"./{_name}")){
                    // return error if it's
                    OutputError($"Project {_name} folder already exist, please create another or change the name\n");
                    return Task.FromResult(1);
                }
                
                // if it's project with authentification 
                if(_auth){
                    
                    _name = $"{companyName}.{projectName}.Auth";
                    // clone the project
                    OutputToConsole($"Add microservice with authentication with name {_name} ... ");
                    runCloneProject(_name, _clean_auth);

                    // rename elements 
                    // OutputToConsole($"Renaming ... ");
                    Directory.SetCurrentDirectory($"../");
                    
                    // // rename user by the name of project
                    // RenameUtility.Rename($"{_name}/", "Auth", name);
                    // RenameUtility.Rename($"{_name}/", "auth", name.ToLower());

                }
                // if it's without authentification
                else{
                    
                    // clone the project
                    OutputToConsole($"Add microservice without authentication with name {_name} ... ");
                    runCloneProject(_name, _clean_sample);
                    Directory.SetCurrentDirectory($"../");
                    // rename  the elements
                    OutputToConsole($"Renaming ... ");
                    // rename Project
                    RenameUtility.Rename($"./{_name}/", "Sample", name);
                    RenameUtility.Rename($"./{_name}/", "Sample", name);
                    // Update Enumeration 
                    RenameUtility.Rename($"./{_name}/", "SAMPLE", name.ToUpper());
                    // update to lower
                    RenameUtility.Rename($"./{_name}/", "sample", name.ToLower());

                }

                // rename company name by default for the real company name
                RenameUtility.Rename($"./{_name}/", "Miccore", companyName);
                // rename project name by default for the real project name
                RenameUtility.Rename($"./{_name}/", "CleanArchitecture", projectName);
                // adding the project to the existing solution
                OutputToConsole($"Adding project to the solution ... ");
                // add api project ot solution
                var process = new Process();
                process.StartInfo.FileName = "dotnet";
                process.StartInfo.Arguments = $"sln {companyName}.{projectName}.sln add ./{_name}/src/{_name}.Api/{_name}.Api.csproj";
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    OutputError(process.StandardError.ReadToEnd());
                    throw new Exception(process.StandardError.ReadToEnd());
                }
                // add api test project to solution
                process.StartInfo.FileName = "dotnet";
                process.StartInfo.Arguments = $"sln {companyName}.{projectName}.sln add ./{_name}/tests/{_name}.IntegrationTest/{_name}.IntegrationTest.csproj";
                process.Start();
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    OutputError(process.StandardError.ReadToEnd());
                    throw new Exception(process.StandardError.ReadToEnd());
                }

                process.StartInfo.FileName = "dotnet";
                process.StartInfo.Arguments = $"sln {companyName}.{projectName}.sln add ./{_name}/tests/{_name}.UnitTest/{_name}.UnitTest.csproj";
                process.Start();
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    OutputError(process.StandardError.ReadToEnd());
                    throw new Exception(process.StandardError.ReadToEnd());
                }

                //rename miccore pagination package
                RenameUtility.Rename($"./{_name}/", $"{companyName}.Net.Pagination", $"Miccore.Net.Pagination");
                RenameUtility.Rename($"./{_name}/", $"{companyName}.Pagination.Model", $"Miccore.Pagination.Model");
                RenameUtility.Rename($"./{_name}/", $"{companyName}.Pagination.Service", $"Miccore.Pagination.Service");

                // buil the solution
                OutputToConsole($"Build the solution ...");
                process.StartInfo.FileName = "dotnet";
                process.StartInfo.Arguments = $"build";
                process.Start();
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    OutputError(process.StandardError.ReadToEnd());
                    throw new Exception(process.StandardError.ReadToEnd());
                }

                // inject the project to the package json file
                OutputToConsole($"Package json injection ... ");
                injection.PackageJsonProjectInject("./package.json", _name, _auth);

                // inject the project to the docker compose file
                OutputToConsole($"Docker compose json injection ... ");
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
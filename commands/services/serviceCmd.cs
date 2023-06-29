using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using miccore.Models;
using miccore.Utility;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace miccore.service
{
    [Command(Name = "service", Description = "manage services")]
    class serviceCmd : miccoreBaseCmd
    {
         [Option("--project | -p",
                Description = "the project name in which you want the service to be created",
                ShowInHelpText = true
                )]
        public string _project {get;}

        [Option(" --name | -n",
                CommandOptionType.SingleValue,
                Description = "The name of the service",
                ShowInHelpText = true)]
        public string _name {get; set; }
        public serviceCmd(ILogger<serviceCmd> logger, IConsole console){
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
                
                if(string.IsNullOrEmpty(_project)){
                    OutputError($"Project name option is required to execute this command, provide project name");
                    return Task.FromResult(1);
                }

                if(string.IsNullOrEmpty(_name)){
                    OutputError($"Name option is required to execute this command, provide name");
                    return Task.FromResult(1);
                }

                if(!Directory.Exists($"./{companyName}.{projectName}.{_project}")){
                    OutputError($"Project {companyName}.{projectName}.{_project} doesn't exist, choose and existing project and don't write the name with the mention");
                    return Task.FromResult(1);
                }
                
                _name = char.ToUpper(_name[0]) + _name.Substring(1).ToLower();
                var current = Path.GetFullPath(".");
                var temp = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
                var name = temp + "/" + _name + (DateTime.Now.ToUniversalTime() - new DateTime (1970, 1, 1)).TotalSeconds;

                Directory.SetCurrentDirectory(temp);
                // if(Directory.Exists(name)){
                //     deleteFolder(name);
                // }
                Directory.CreateDirectory(name);

                OutputToConsole($"Add new service with name {_name} to {_project} project ... \n");
                runOnlyClone(name, _clean_items);

                // set folder as normal 
                setNormalFolder(name);

                //copy of different elements
                var proj = $"{companyName}.{projectName}.{_project}";
                var path = $"{current}/{companyName}.{projectName}.{_project}/src";
                var test = $"{current}/{companyName}.{projectName}.{_project}/tests";
                // Api controller
                OutputToConsole($"Generate Api Controller \n");
                DirectoryCopy($"{name}/Miccore.CleanArchitecture.Sample.Api/Controllers", $"{path}/{proj}.Api/Controllers", true);
                // Api validators
                OutputToConsole($"Generate Api Validators \n");
                DirectoryCopy($"{name}/Miccore.CleanArchitecture.Sample.Api/Validators", $"{path}/{proj}.Api/Validators", true);
                // Application Commands
                OutputToConsole($"Generate Application Commands \n");
                DirectoryCopy($"{name}/Miccore.CleanArchitecture.Sample.Application/Commands", $"{path}/{proj}.Application/Commands", true);
                // Application Handlers
                OutputToConsole($"Generate Application Handlers \n");
                DirectoryCopy($"{name}/Miccore.CleanArchitecture.Sample.Application/Handlers", $"{path}/{proj}.Application/Handlers", true);
                // Application Mappers
                OutputToConsole($"Generate Application Mappers \n");
                DirectoryCopy($"{name}/Miccore.CleanArchitecture.Sample.Application/Mappers", $"{path}/{proj}.Application/Mappers", true);
                // Application Queries
                OutputToConsole($"Generate Application Queries \n");
                DirectoryCopy($"{name}/Miccore.CleanArchitecture.Sample.Application/Queries", $"{path}/{proj}.Application/Queries", true);
                // Application Responses
                OutputToConsole($"Generate Application Responses \n");
                DirectoryCopy($"{name}/Miccore.CleanArchitecture.Sample.Application/Responses", $"{path}/{proj}.Application/Responses", true);
                // core Entities
                OutputToConsole($"Generate core Entities \n");
                DirectoryCopy($"{name}/Miccore.CleanArchitecture.Sample.Core/Entities", $"{path}/{proj}.Core/Entities", true);
                // core Repositories
                OutputToConsole($"Generate core Repositories \n");
                DirectoryCopy($"{name}/Miccore.CleanArchitecture.Sample.Core/Repositories", $"{path}/{proj}.Core/Repositories", true);
                // Infrastructure Repositories
                OutputToConsole($"Generate Infrastructure Repositories \n");
                DirectoryCopy($"{name}/Miccore.CleanArchitecture.Sample.Infrastructure/Repositories", $"{path}/{proj}.Infrastructure/Repositories", true);
                // unit test
                OutputToConsole($"Generate unit test \n");
                DirectoryCopy($"{name}/Miccore.CleanArchitecture.Sample.UnitTest/Sample", $"{test}/{proj}.UnitTest/Sample", true);

                OutputToConsole($"Renaming samples \n");
                Directory.SetCurrentDirectory(current);
                // rename company name by default for the real company name
                RenameUtility.Rename($"./{proj}/", "Miccore.CleanArchitecture.Sample", proj);
                // rename database context name
                RenameUtility.Rename($"./{proj}/", "SampleApplicationDbContext", $"{_project}ApplicationDbContext");
                // RenameUtility.Rename($"{_name}/", "Sample.Core", $"{_project}.Core");
                RenameUtility.Rename($"./{proj}/", "Sample", _name);
                // Update Enumeration 
                RenameUtility.Rename($"./{proj}/", "SAMPLE", _name.ToUpper());
                // update to lower
                RenameUtility.Rename($"./{proj}/", "sample", _name.ToLower());

                // injection 
                OutputToConsole($"Dependencies injections ... \n");
                InjectionUtility injection = new InjectionUtility(_logger);
                // core enumeration
                OutputToConsole($"Core Enumeration injections \n");
                injection.CoreEnumerationInject($"{path}/{proj}.Core/Enumerations/ExceptionEnum.cs", _name);

                // infrastructure dbcontext injection
                OutputToConsole($"Infrastructure db context injections \n");
                injection.InfrastructureDbContextInject($"{path}/{proj}.Infrastructure/Data/{_project}ApplicationDbContext.cs", proj, _name);

                // infrastructure service injection
                OutputToConsole($"Infrastructure service injections \n");
                injection.InfrastructureServiceInject($"{path}/{proj}.Infrastructure/Persistances/DependencyInjection.cs", proj, _name);

                // Package json service injection
                OutputToConsole($"Package json service injection ... \n");
                injection.PackageJsonReferenceServiceInject($"{current}/package.json", _project, _name);

                // ocelot project service injection
                OutputToConsole($"Ocelot project service injection ... \n");
                injection.OcelotProjectServiceInjection($"{current}/package.json",$"{current}/{proj}/configuration.json", _project, _name);

                // delete temp file
                Directory.SetCurrentDirectory(current);
                deleteFolder(name);

                // build solution
                OutputToConsole($"Build Solution \n");
                process.StartInfo.FileName = "dotnet";
                process.StartInfo.Arguments = $"build";
                process.Start();
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    OutputError(process.StandardError.ReadToEnd());
                    throw new Exception(process.StandardError.ReadToEnd());
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
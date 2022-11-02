using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using miccore.Models;
using miccore.Utility;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace miccore.project
{
    [Command(Name = "project", Description = "delete project")]
    class deleteProjectCmd : miccoreBaseCmd
    {
         
        [Option(" --name | -n",
                CommandOptionType.SingleValue,
                Description = "The name of the project",
                ShowInHelpText = true)]
        public string _name {get; set; }
        public deleteProjectCmd(ILogger<deleteProjectCmd> logger, IConsole console){
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
                    OutputError("microservice solution not foundgo to the general project");
                    return Task.FromResult(1);
                }

                // check if name is given
                if(string.IsNullOrEmpty(_name)){
                    // return error if not
                    OutputError($"name option is required to execute this command");
                    return Task.FromResult(1);
                }

                // parse the name to camel case
                _name = char.ToUpper(_name[0]) + _name.Substring(1).ToLower();
                var name = _name;
                _name = $"{companyName}.{projectName}.{_name}";

                // remove project to package json
                OutputToConsole($"Remove project to package json ...");
                injection.PackageJsonRemoveProject("./package.json", _name);

                // remove the project to the docker compose file
                OutputToConsole($"Docker compose json remove ... ");
                injection.DockerProjectRemove("./docker-compose.yml", name.ToLower());

                // remove project to solution 
                OutputToConsole($"Remove project to the solution ...");
                var process = new Process();
                process.StartInfo.FileName = "dotnet";
                process.StartInfo.Arguments = $"sln {companyName}.{projectName}.sln remove ./{_name}/src/{_name}.Api/{_name}.Api.csproj";
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

                // remove integration test to solution
                OutputToConsole($"  remove integration test project to the solution ...");
                process.StartInfo.FileName = "dotnet";
                process.StartInfo.Arguments = $"sln {companyName}.{projectName}.sln remove ./{_name}/tests/{_name}.IntegrationTest/{_name}.IntegrationTest.csproj";
                process.Start();
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    OutputError(process.StandardError.ReadToEnd());
                    throw new Exception(process.StandardError.ReadToEnd());
                }

                // remove unit test to solution
                OutputToConsole($"remove unit test project to the solution ...");
                process.StartInfo.FileName = "dotnet";
                process.StartInfo.Arguments = $"sln {companyName}.{projectName}.sln remove ./{_name}/tests/{_name}.UnitTest/{_name}.UnitTest.csproj";
                process.Start();
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    OutputError(process.StandardError.ReadToEnd());
                    throw new Exception(process.StandardError.ReadToEnd());
                }

                // delete project
                OutputToConsole($"delete microservice project name name {_name} ...");
                deleteFolder($"./{_name}");

                // build the solution
                OutputToConsole($"  build the solution ...");
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
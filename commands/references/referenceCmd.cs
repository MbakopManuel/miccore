using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using miccore.Models;
using miccore.Utility;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace miccore.reference
{
    [Command(Name = "reference", Description = "manage reference from another project to another one")]
    class referenceCmd : miccoreBaseCmd
    {
         [Option("--from | -f",
                Description = "project name of the project which will be refered",
                ShowInHelpText = true
                )]
        public string _from {get;}

        [Option(" --to | -t",
                CommandOptionType.SingleValue,
                Description = "The name of the project where the reference will be applied",
                ShowInHelpText = true)]
        public string _to {get; set; }
        public referenceCmd(ILogger<referenceCmd> logger, IConsole console){
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

                // check if from is given
                if(string.IsNullOrEmpty(_from)){
                    // return error if not
                    OutputError($"From option is required to execute this command");
                    return Task.FromResult(1);
                }

                // check if to is given
                if(string.IsNullOrEmpty(_to)){
                    // return error if not
                    OutputError($"To option is required to execute this command");
                    return Task.FromResult(1);
                }
               
                OutputToConsole($"Reference adding ...");
                var from = $"{companyName}.{projectName}.{_from}/src/{companyName}.{projectName}.{_from}.Api/{companyName}.{projectName}.{_from}.Api.csproj";
                var to = $"{companyName}.{projectName}.{_to}/src/{companyName}.{projectName}.{_to}.Api/{companyName}.{projectName}.{_to}.Api.csproj";
                
                if(!Directory.Exists($"./{companyName}.{projectName}.{_from}")){
                    OutputError($"\nProject {companyName}.{projectName}.{_from} doesn't exist, choose and existing project and don't write the name with the mention\n\n");
                    return Task.FromResult(1);
                }
                
                if(!Directory.Exists($"./{companyName}.{projectName}.{_to}")){
                    OutputError($"\nProject {companyName}.{projectName}.{_to} doesn't exist, choose and existing project and don't write the name with the mention\n\n");
                    return Task.FromResult(1);
                }

                process.StartInfo.FileName = "dotnet";
                process.StartInfo.Arguments = $"add {to} reference {from}";
                process.Start();
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    throw new Exception(process.StandardError.ReadToEnd());
                }

                InjectionUtility injection = new InjectionUtility(_logger);

                OutputToConsole($"Package json reference injection ... ");
                injection.PackageJsonReferenceInject("./package.json", _to, _from);

    //         OutputToConsole($"Mapper profiles injections  ...");
    //         injection.ServiceNameSpacesImportationForReference($"./package.json", $"./{_to}.Api/{_to}.Api/Services/Services.cs", _from);
    //         injection.ServiceProfileAddingForReference($"./package.json",$"./{_to}.Api/{_to}.Api/Services/Services.cs", _from);

                OutputToConsole($"Docker reference injection ...");
                injection.DockerReferenceInjection("./docker-compose.yml", _to);

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
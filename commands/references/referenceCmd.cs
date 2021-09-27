using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using miccore.Utility;
using Microsoft.Extensions.Logging;

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
            try
            {
               
               if(File.Exists("./Microservice.WebApi.sln")){
                    
                    if(!File.Exists("./package.json")){
                        OutputError("breaking project, package.json file doesn't exist\n");
                        return Task.FromResult(1);
                    }
                    OutputToConsole($" \n******************************************************************************************** \n");
                    OutputToConsole($"   Reference adding ... \n");
                    OutputToConsole($" \n******************************************************************************************** \n\n");

                    var from = $"{_from}.Microservice/{_from}.Microservice/{_from}.Microservice.csproj";
                    var to = $"{_to}.Microservice/{_to}.Microservice/{_to}.Microservice.csproj";
                    var process = Process.Start("dotnet", $"add {to} reference {from}");
                    process.WaitForExit();

                    InjectionUtility injection = new InjectionUtility();

                    OutputToConsole($" \n******************************************************************************************** \n");
                    OutputToConsole($"   package json reference injection ... \n");
                    OutputToConsole($" \n******************************************************************************************** \n\n");
                    injection.PackageJsonReferenceInject("./package.json", _to, _from);


                    OutputToConsole($" \n******************************************************************************************** \n");
                    OutputToConsole($"   mapper profiles injections  ... \n");
                    OutputToConsole($" \n******************************************************************************************** \n\n");
                    injection.ServiceNameSpacesImportationForReference($"./package.json", $"./{_to}.Microservice/{_to}.Microservice/Services/Services.cs", _from);
                    injection.ServiceProfileAddingForReference($"./package.json",$"./{_to}.Microservice/{_to}.Microservice/Services/Services.cs", _from);


                    OutputToConsole($" \n******************************************************************************************** \n");
                    OutputToConsole($"   docker reference injection ... \n");
                    OutputToConsole($" \n******************************************************************************************** \n\n");
                    injection.DockerReferenceInjection("./docker-compose.yml", _to);


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
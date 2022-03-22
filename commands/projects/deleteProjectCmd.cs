using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using miccore.Utility;
using Microsoft.Extensions.Logging;

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
        public deleteProjectCmd(ILogger<projectCmd> logger, IConsole console){
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
                    
                    OutputToConsole($" \n******************************************************************************************** \n");
                    OutputToConsole($"   package and ocelot project remove ... \n");
                    OutputToConsole($" \n******************************************************************************************** \n\n");
                    InjectionUtility injection = new InjectionUtility();
                    injection.ProjectDeletion("./Gateway.WebApi/ocelot.json", _name);

                    OutputToConsole($" \n******************************************************************************************** \n");
                    OutputToConsole($"  remove project to the solution ... \n");
                    OutputToConsole($" \n******************************************************************************************** \n\n");
                    var process = Process.Start("dotnet", $"sln Microservice.WebApi.sln remove ./{_name}.Microservice/{_name}.Microservice.csproj");
                    process.WaitForExit();
                    
                    if(process.ExitCode != 0){
                        throw new Exception(process.StandardError.ReadLine());
                    }

                    OutputToConsole($" \n******************************************************************************************** \n");
                    OutputToConsole($"   delete microservice project name name {_name} ... \n");
                    OutputToConsole($" \n******************************************************************************************** \n\n");
                    deleteFolder($"./{_name}.Microservice/");

                    OutputToConsole($" \n******************************************************************************************** \n");
                    OutputToConsole($"  build the solution ... \n");
                    OutputToConsole($" \n******************************************************************************************** \n\n");
                    process = Process.Start("dotnet", $"build");
                    process.WaitForExit();
                    
                    if(process.ExitCode != 0){
                        throw new Exception(process.StandardError.ReadLine());
                    }

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
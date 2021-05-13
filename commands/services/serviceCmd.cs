using System;
using System.IO;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

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

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            try
            {
                if(string.IsNullOrEmpty(_project)){
                    OutputError($"\n project name option is required to execute this command, provide project name\n\n");
                }

                if(string.IsNullOrEmpty(_name)){
                    OutputError($"\n name option is required to execute this command, provide name\n\n");
                }

                if(!Directory.Exists($"./{_project}.Microservice")){
                    OutputError($"\nProject {_project} doesn't exist, choose and existing project and don't write the name with the mention .Microservice\n\n");
                }
                
                _name = char.ToUpper(_name[0]) + _name.Substring(1).ToLower();
                var current = Path.GetFullPath(".");
                var temp = Path.GetTempPath();
                var name = temp + _name;

                // Directory.CreateDirectory(name);
                Directory.SetCurrentDirectory(temp);

                OutputToConsole($" \n******************************************************************************************** \n");
                OutputToConsole($"   add new service with name {_name} to {_project} project ... \n");
                OutputToConsole($" \n******************************************************************************************** \n\n");
                runOnlyClone(_name, _source_samples_services);

                setNormalFolder(name);
                RenameUtility rename = new RenameUtility();

                File.Copy($"{name}/samples-operation", $"{current}/{_project}.Microservice/Operations/{_name}", true);
                // rename.Rename($"{current}/{_project}.Microservice", "samples-operation", _name);

                File.Copy($"{name}/samples-service", $"{current}/{_project}.Microservice/Services/{_name}", true);
                // rename.Rename($"{current}/{_project}.Microservice", "samples-service", _name);

                File.Copy($"{name}/samples-repository", $"{current}/{_project}.Microservice/Repositories/{_name}", true);
                // rename.Rename($"{current}/{_project}.Microservice", "samples-repository", _name);

                rename.Rename($"{current}/{_project}.Microservice", "Sample", _name);
                rename.Rename($"{current}/{_project}.Microservice", "sample",  char.ToLower(_name[0]) + _name.Substring(1).ToLower());

               
                Directory.SetCurrentDirectory(current);
                deleteFolder(name);
                
                return 0;
            }
            catch (Exception ex)
            {
                OnException(ex);
                return 1;
            }
        }

    }
}
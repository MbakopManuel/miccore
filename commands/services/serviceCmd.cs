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
            try
            {
                if(string.IsNullOrEmpty(_project)){
                    OutputError($"\n project name option is required to execute this command, provide project name\n\n");
                    return Task.FromResult(1);
                }

                if(string.IsNullOrEmpty(_name)){
                    OutputError($"\n name option is required to execute this command, provide name\n\n");
                    return Task.FromResult(1);
                }

                if(!Directory.Exists($"./{_project}.Api")){
                    OutputError($"\nProject {_project} doesn't exist, choose and existing project and don't write the name with the mention .Api\n\n");
                    return Task.FromResult(1);
                }

                // check if package json file exist
                if(!File.Exists("./package.json")){
                    OutputError("\n\nError: Package file not found\n\n");
                    return Task.FromResult(1);
                }
                // get the company name to the package json file
                var text = File.ReadAllText("./package.json");
                Package package = JsonConvert.DeserializeObject<Package>(text);
                // company name
                string companyName = package.CompanyName;
                // project name
                string projectName = package.Name;
                
                _name = char.ToUpper(_name[0]) + _name.Substring(1).ToLower();
                var current = Path.GetFullPath(".");
                var temp = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
                var name = temp + "/" + _name;

                // Directory.CreateDirectory(name);
                Directory.SetCurrentDirectory(temp);

                OutputToConsole($" \n******************************************************************************************** \n");
                OutputToConsole($"   add new service with name {_name} to {_project} project ... \n");
                OutputToConsole($" \n******************************************************************************************** \n\n");
                runOnlyClone(_name, _source_samples_services);

                setNormalFolder(name);
    
                DirectoryCopy($"{name}/samples-operation", $"{current}/{_project}.Api/{_project}.Api/Operations/{_name}", true);
                // rename.Rename($"{current}/{_project}.Api", "samples-operation", _name);

                DirectoryCopy($"{name}/samples-service", $"{current}/{_project}.Api/{_project}.Api/Services/{_name}", true);
                // rename.Rename($"{current}/{_project}.Api", "samples-service", _name);

                DirectoryCopy($"{name}/samples-repository", $"{current}/{_project}.Api/{_project}.Api/Repositories/{_name}", true);
                // rename.Rename($"{current}/{_project}.Api", "samples-repository", _name);

                RenameUtility rename = new RenameUtility();
                rename.Rename($"{current}/{_project}.Api", "Miccore.Net", companyName);
                rename.Rename($"{current}/{_project}.Api", "webapi_template", projectName);
                rename.Rename($"{current}/{_project}.Api", "Sample", _name);
                rename.Rename($"{current}/{_project}.Api", "sample",  char.ToLower(_name[0]) + _name.Substring(1).ToLower());
                rename.Rename($"{current}/{_project}.Api", $"{_name}.Api", $"{_project}.Api");


                OutputToConsole($" \n******************************************************************************************** \n");
                OutputToConsole($"   dependencies injections ... \n");
                OutputToConsole($" \n******************************************************************************************** \n\n");

                InjectionUtility injection = new InjectionUtility();
                OutputToConsole($"   services and repositories injections ... \n");
                injection.ServiceNameSpacesImportation($"{current}/{_project}.Api/{_project}.Api/Services/Services.cs", $"{companyName}.{projectName}.{_project}.Api", _name);
                injection.ServiceRepositoryServicesInjection($"{current}/{_project}.Api/{_project}.Api/Services/Services.cs", _name);
                injection.ServiceProfileAdding($"{current}/{_project}.Api/{_project}.Api/Services/Services.cs", _name);
                
                OutputToConsole($"   DBContext model creations ... \n");
                injection.DBContextNameSpacesImportation($"{current}/{_project}.Api/{_project}.Api/Data/IApplicationDbContext.cs", $"{companyName}.{projectName}.{_project}.Api", _name);
                injection.DBContextIApplicationInjection($"{current}/{_project}.Api/{_project}.Api/Data/IApplicationDbContext.cs",  _name);

                injection.DBContextNameSpacesImportation($"{current}/{_project}.Api/{_project}.Api/Data/ApplicationDbContext.cs", $"{companyName}.{projectName}.{_project}.Api", _name);
                injection.DBContextApplicationInjection($"{current}/{_project}.Api/{_project}.Api/Data/ApplicationDbContext.cs",  _name);
                
                
                OutputToConsole($"   ocelot project service injection ... \n");
                injection.OcelotProjectServiceInjection($"{current}/package.json",$"{current}/Gateway.WebApi/ocelot.json", _project, _name);


                OutputToConsole($"   package json service injection ... \n");
                injection.PackageJsonReferenceServiceInject($"{current}/package.json", _project, _name);


                OutputToConsole($" \n******************************************************************************************** \n");
                OutputToConsole($"   solution building ... \n");
                OutputToConsole($" \n******************************************************************************************** \n\n");

                Directory.SetCurrentDirectory(current);
                deleteFolder(name);

                var process1 = Process.Start("dotnet", "build");
                process1.WaitForExit();
                if (process1.ExitCode != 0)
                {
                    throw new Exception(process1.StandardError.ReadLine());
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
using System;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using miccore.Utility;
using Microsoft.Extensions.Logging;

namespace miccore
{
    [Command(Name = "new", Description = "new Web Microservice Api architecture with ocelot gateway")]
    class newCmd : miccoreBaseCmd
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

        [Option(" --company | -c",
                CommandOptionType.SingleValue,
                Description = "The name of the company",
                ShowInHelpText = true)]
        public string _companyName {get; set; }

        [Option(" --project | -p",
                CommandOptionType.SingleValue,
                Description = "The type of project, we have two types (webapi or xamarin). default: webapi",
                ShowInHelpText = true)]
        public string _project {get; set; }

        public newCmd(ILogger<newCmd> logger, IConsole console){
            _logger = logger;
            _console = console;
        }

        protected override Task<int> OnExecute(CommandLineApplication app)
        {

            // create a new project
            RenameUtility rename = new RenameUtility();
            try
            {
                // check if  project type is not empty
                if(string.IsNullOrEmpty(_project)){
                    // if it is, set project type to webapi by default
                    _project = "webapi";
                    // set the default name to MiccroserviceWebApi
                    _name = "MiccoreWebApi";
                    // if with want integrate in the project auth packages
                    if(_auth){
                        // create auth micro service webapi project            
                        OutputToConsole($" \n******************************************************************************************** \n");
                        OutputToConsole($" {_project} project creation with authentication with name {_name} ...\n");
                        OutputToConsole($" \n******************************************************************************************** \n\n");
                        // cloning the template from github
                        runClone(_name, _source_with_auth);
                        rename.Rename($"./", "webapi_template", _name);
                        if(!string.IsNullOrEmpty(_companyName)) rename.Rename($"./", "Miccore.Net", _companyName);

                        return Task.FromResult(0);

                    }
                    // create a simple microservice webapi without auth
                    OutputToConsole($" \n******************************************************************************************** \n");
                    OutputToConsole($" {_project} project without authentication with name {_name} ...\n");
                    OutputToConsole($" \n******************************************************************************************** \n\n");
                    // cloning the template from github
                    runClone(_name, _source_without_auth); 
                    rename.Rename($"./", "webapi_template", _name);
                    if(!string.IsNullOrEmpty(_companyName)) rename.Rename($"./", "Miccore.Net", _companyName);

                    return Task.FromResult(0);

                }

                // check the project type
                switch (_project)
                {
                    // if it is webapi
                    case "webapi":
                        // check if the name is empty and set the default name if it's
                        if(string.IsNullOrEmpty(_name)){
                            _name = "MiccoreWebApi";
                        }    

                        // if with want integrate in the project auth packages
                        if(_auth){
                            // create auth micro service webapi project  
                            OutputToConsole($" \n******************************************************************************************** \n");
                            OutputToConsole($" {_project} project creation with authentication with name {_name} ...\n");
                            OutputToConsole($" \n******************************************************************************************** \n\n");
                            // cloning the template from github
                            runClone(_name, _source_with_auth);
                            // rename in the project webapi_template by the name of project
                            rename.Rename($"./", "webapi_template", _name);
                            if(!string.IsNullOrEmpty(_companyName)) rename.Rename($"./", "Miccore.Net", _companyName);
                            break;

                        }
                        // create a simple microservice webapi without auth
                        OutputToConsole($" \n******************************************************************************************** \n");
                        OutputToConsole($" {_project} project without authentication with name {_name} ...\n");
                        OutputToConsole($" \n******************************************************************************************** \n\n");
                        runClone(_name, _source_without_auth);
                        // rename in the project webapi_template by the name of project
                        rename.Rename($"./", "webapi_template", _name);
                        if(!string.IsNullOrEmpty(_companyName)) rename.Rename($"./", "Miccore.Net", _companyName);

                    break;
                    // by default create xamarin project if it's not webapi
                    default:
                        // check if the name is empty and set the default name if it's
                        if(string.IsNullOrEmpty(_name)){
                            _name = "MiccoreXamarinMobileApp";
                        }
                         // create xamarin project
                        OutputToConsole($" \n******************************************************************************************** \n");
                        OutputToConsole($" {_project} project creation with name {_name} ... \n");
                        OutputToConsole($" \n******************************************************************************************** \n\n");
                        // cloning the template from github
                        runClone(_name, _source_xamarin);
                    
                    break;
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
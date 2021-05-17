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

        [Option(" --project | -p",
                CommandOptionType.SingleValue,
                Description = "The type of project, we have two types (webapi or xamarin). default: webapi",
                ShowInHelpText = true)]
        public string _project {get; set; }

        public newCmd(ILogger<newCmd> logger, IConsole console){
            _logger = logger;
            _console = console;
        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            try
            {
                if(string.IsNullOrEmpty(_project)){
                    _project = "webapi";
                    _name = "MiccoreWebApi";
                   
                    if(_auth){
                                
                        OutputToConsole($" \n******************************************************************************************** \n");
                        OutputToConsole($" {_project} project creation with authentication with name {_name} ...\n");
                        OutputToConsole($" \n******************************************************************************************** \n\n");
                        runClone(_name, _source_with_auth);

                    }else{

                        OutputToConsole($" \n******************************************************************************************** \n");
                        OutputToConsole($" {_project} project without authentication with name {_name} ...\n");
                        OutputToConsole($" \n******************************************************************************************** \n\n");
                        runClone(_name, _source_without_auth);
                    }    

                }else{
                    switch (_project)
                    {
                        case "webapi":

                            if(string.IsNullOrEmpty(_name)){
                                _name = "MiccoreWebApi";
                            }      

                            if(_auth){
                                
                                OutputToConsole($" \n******************************************************************************************** \n");
                                OutputToConsole($" {_project} project creation with authentication with name {_name} ...\n");
                                OutputToConsole($" \n******************************************************************************************** \n\n");
                                runClone(_name, _source_with_auth);

                            }else{
 
                                OutputToConsole($" \n******************************************************************************************** \n");
                                OutputToConsole($" {_project} project without authentication with name {_name} ...\n");
                                OutputToConsole($" \n******************************************************************************************** \n\n");
                                runClone(_name, _source_without_auth);
                            }     

                            RenameUtility rename = new RenameUtility();
                            rename.Rename($"./", "webapi_template", _name);

                        break;
                        default:

                            if(string.IsNullOrEmpty(_name)){
                                _name = "MiccoreXamarinMobileApp";
                            }
                            
                            OutputToConsole($" \n******************************************************************************************** \n");
                            OutputToConsole($" {_project} project creation with name {_name} ... \n");
                            OutputToConsole($" \n******************************************************************************************** \n\n");
                        
                        break;
                    }
                }

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
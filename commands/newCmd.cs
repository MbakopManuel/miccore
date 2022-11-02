using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using miccore.Utility;
using Microsoft.Extensions.Logging;

namespace miccore
{
    [Command(Name = "new", Description = "new Web Microservice Api architecture with ocelot gateway")]
    class newCmd : miccoreBaseCmd
    {

        // [Option("--with-auth | -wa",
        //         Description = "create a project with authentication or not, if specified your project will be created with an authentication microservice and otherwise it will be created with a sample microservice called sample",
        //         ShowInHelpText = true
        //         )]
        // public bool _auth {get;}

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

        public newCmd(ILogger<newCmd> logger, IConsole console){
            _logger = logger;
            _console = console;
        }

        protected override Task<int> OnExecute(CommandLineApplication app)
        {
            // create a new project
            var process = new Process();
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            try
            {
                // set the default name to MiccroserviceWebApi
                if(string.IsNullOrEmpty(_name)) _name = "MiccoreWebApi";

                // create a simple microservice webapi without auth
                OutputToConsole($"Clean Architecture Miccore Webapi Project {_name} ...");
                
                // cloning the template from github
                runClone(_name, _clean_project); 
                RenameUtility.Rename($"./", "CleanArchitecture", _name);
                RenameUtility.Rename($"./", "cleanarchitecture", _name.ToLower());
                if(!string.IsNullOrEmpty(_companyName)){
                     RenameUtility.Rename($"./", "Miccore", _companyName);
                     RenameUtility.Rename($"./", "miccore", _companyName.ToLower());
                }

                // build project
                process.StartInfo.FileName = "dotnet";
                process.StartInfo.Arguments = "build";
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